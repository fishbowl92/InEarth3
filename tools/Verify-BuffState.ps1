param(
    # 기본값은 같은 저장소의 소스입니다. 수정 전 파일을 지정하면 오류를 재현할 수 있습니다.
    [string]$MapManagerPath = (Join-Path (Split-Path -Parent $PSScriptRoot) 'MapManager.cs')
)
$ErrorActionPreference = 'Stop'

# 실제 MapManager.cs에서 대상 메서드를 추출합니다. 테스트에 구현을 복사해 두지 않습니다.
# Unity가 없는 보존 저장소이므로 벡터와 화면 참조는 최소 대역으로 대체합니다.
$sourceText = Get-Content -LiteralPath $MapManagerPath -Raw -Encoding UTF8
$enumMatch = [regex]::Match($sourceText, '(?s)public enum buffState\s*\{.*?\}')
if (-not $enumMatch.Success) { throw 'buffState 열거형을 찾지 못했습니다.' }

function Get-SourceMethod {
    param([string]$MethodName)
    # 메서드의 네 칸 들여쓰기 닫는 괄호까지 읽습니다. 형식 변경 시 조용히 건너뛰지 않고 실패합니다.
    $pattern = '(?ms)^    public (?:int|Vector2Int) ' + [regex]::Escape($MethodName) + '\([^\r\n]*\).*?^    \}'
    $match = [regex]::Match($sourceText, $pattern)
    if (-not $match.Success) { throw "검증할 메서드를 찾지 못했습니다: $MethodName" }
    return $match.Value
}

$methods = @('setBuffStateGab', 'removeBuffStateGab', 'getBuffStateGab', 'buffStateCodeFilter', 'manageBuff') |
    ForEach-Object { Get-SourceMethod -MethodName $_ }

$testSource = @'
using System;
using System.Collections.Generic;

// Unity의 Vector2Int 중 테스트가 사용하는 두 정수와 zero만 제공합니다.
public struct Vector2Int {
    public int x, y;
    public Vector2Int(int x, int y) { this.x = x; this.y = y; }
    public static Vector2Int zero { get { return new Vector2Int(0, 0); } }
}
public class BuffStateRegressionProbe {
    __ENUM__
    public class UiReference { public object parent; }
    public class BuffEntry {
        public buffState buffSort;
        public int count;
        public UiReference myUI = new UiReference();
    }
    public int[] BuffState = new int[3];
    public List<BuffEntry>[] buffListNew = {
        new List<BuffEntry>(), new List<BuffEntry>(), new List<BuffEntry>()
    };
    public object[] buffSetInvPenal = { new object(), new object(), new object() };
    __METHODS__
    private static int checks;
    private static void Check(bool condition, string message) {
        checks++;
        if (!condition) throw new Exception(message);
    }
    public static string Run() {
        checks = 0;
        var probe = new BuffStateRegressionProbe();
        var types = (buffState[])Enum.GetValues(typeof(buffState));
        int allFlags = 0;
        foreach (var type in types) allFlags = probe.setBuffStateGab(allFlags, type);
        foreach (var type in types) {
            int one = probe.setBuffStateGab(0, type);
            Check(probe.getBuffStateGab(one, type) == 1, "Activation failed: " + type);
            int cleared = probe.removeBuffStateGab(one, type);
            Check(probe.getBuffStateGab(cleared, type) == 0, "Removal failed: " + type);
            Check(probe.removeBuffStateGab(cleared, type) == cleared, "Repeated removal changed state");
            int remaining = probe.removeBuffStateGab(allFlags, type);
            foreach (var other in types) {
                Check(probe.getBuffStateGab(remaining, other) == (other == type ? 0 : 1),
                    "Removal changed another buff: " + type + " / " + other);
            }
        }
        // 대상 인덱스를 비트 상태로 잘못 넘기거나, kind=2가 삭제 전에 반환하는 오류를 검증합니다.
        for (int target = 0; target < 3; target++) {
            for (int kind = 0; kind <= 2; kind++) {
                var state = new BuffStateRegressionProbe();
                var removed = new BuffEntry { buffSort = buffState.Atk, count = 7 };
                var kept = new BuffEntry { buffSort = buffState.Dex, count = 3 };
                state.buffListNew[target].Add(removed);
                state.buffListNew[target].Add(kept);
                state.BuffState[target] = state.setBuffStateGab(
                    state.setBuffStateGab(0, buffState.Atk), buffState.Dex);
                int value = state.manageBuff(buffState.Atk, target, kind);
                Check(value == (kind == 1 ? 0 : 7), "Wrong count returned");
                Check(state.getBuffStateGab(state.BuffState[target], buffState.Atk) == (kind == 0 ? 1 : 0),
                    "Target buff state was not updated");
                Check(state.getBuffStateGab(state.BuffState[target], buffState.Dex) == 1,
                    "Unrelated buff was lost");
                Check(state.buffListNew[target].Count == (kind == 0 ? 2 : 1), "Wrong list count");
                if (kind != 0) {
                    Check(ReferenceEquals(removed.myUI.parent, state.buffSetInvPenal[target]),
                        "Removed UI was not returned");
                    Check(ReferenceEquals(state.buffListNew[target][0], kept), "Wrong entry removed");
                    Check(state.manageBuff(buffState.Atk, target, kind) == 0, "Repeated removal changed result");
                }
            }
        }
        return "PASS: " + checks + " assertions, " + types.Length + " buff types, 3 targets, 3 modes.";
    }
}
'@
$testSource = $testSource.Replace('__ENUM__', $enumMatch.Value).Replace('__METHODS__', ($methods -join [Environment]::NewLine))
# 원본의 비어 있는 catch에서 사용하지 않는 변수가 있으므로 해당 경고만 허용합니다.
Add-Type -TypeDefinition $testSource -CompilerOptions '/nowarn:0168'
[BuffStateRegressionProbe]::Run()
