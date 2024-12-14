# Auto Path Generator

Unity 에디터에서 Resources 폴더와 Addressables(선택사항)의 경로를 자동으로 상수 문자열로 생성해주는 도구입니다. 이 도구는 Unity 프로젝트에서 하드코딩된 문자열 경로 사용을 방지하고, 컴파일 타임에 리소스 경로의 안전성을 보장합니다.

## 주요 기능

- 🔄 Resources 폴더의 경로를 자동으로 상수화
- 🎯 Addressables 지원 (선택사항)
- ⚙️ 생성된 파일의 출력 경로 설정 가능
- 🔄 에셋 변경 시 자동 생성 옵션
- 🛡️ 리소스 경로의 컴파일 타임 안전성 보장
- 🧹 특수 문자 처리를 포함한 깔끔한 경로명 생성

## 설치 방법

### Package Manager를 통한 설치

Unity 2019.3.4f1 이상 버전에서는 Package Manager에서 직접 Git URL을 통해 설치할 수 있습니다.

1. Package Manager 창을 엽니다 (Window > Package Manager)
2. '+' 버튼을 클릭하고 "Add package from git URL"을 선택합니다
3. 다음 URL을 입력합니다:
```
https://github.com/jinhosung96/AutoPathGeneratorForUnity.git
```

또는 `Packages/manifest.json` 파일에 직접 추가할 수 있습니다:
```json
{
  "dependencies": {
    "com.jhs-library.auto-path-generator": "https://github.com/jinhosung96/AutoPathGeneratorForUnity.git"
  }
}
```

특정 버전을 설치하려면 URL 뒤에 #{version} 태그를 추가하면 됩니다:
```
https://github.com/jinhosung96/AutoPathGeneratorForUnity.git#1.0.0
```
## 사용 방법

### 설정

1. `Tools > Auto Path Generator > Settings` 메뉴를 통해 설정에 접근
2. 다음 옵션들을 설정할 수 있습니다:
   - **출력 경로**: 생성된 경로 상수가 저장될 디렉토리 (Assets 폴더 기준 상대 경로)
   - **자동 생성**: 에셋 변경 시 자동 생성 여부 설정

### 수동 생성

경로 상수를 수동으로 생성하는 방법:
1. `Tools > Auto Path Generator > Generate` 메뉴 사용
2. 또는 설정 에셋을 선택하고 "Generate" 버튼 클릭

### 생성된 경로 사용하기

생성된 경로는 `JHS.Library.AutoPathGenerator` 네임스페이스에서 사용할 수 있습니다. 사용 예시:

```csharp
// 문자열 리터럴 사용 대신
var prefab = Resources.Load("Prefabs/Characters/Player");

// 생성된 상수 사용
var prefab = Resources.Load(ResourcesPaths.Prefabs_Characters_Player);
```

## 생성되는 코드 구조

이 도구는 모든 리소스 경로에 대한 상수 문자열을 포함하는 정적 클래스를 생성합니다:

```csharp
namespace JHS.Library.AutoPathGenerator
{
    public static class ResourcesPaths
    {
        public const string Example_Path = "Example/Path";
        // ... 기타 경로들
    }
    
    public static class AddressablePaths // Addressables 지원이 활성화된 경우
    {
        public const string Example_Address = "Example/Address";
        // ... 기타 주소들
    }
}
```

## 특수 경로 처리

- 공백은 언더스코어로 대체
- 슬래시(/, \)는 언더스코어로 변환
- 특수 문자 제거
- 경로명 시작의 숫자는 언더스코어로 접두
- C# 예약어는 @ 기호로 접두
- 연속된 언더스코어는 하나로 통합

## Addressables 지원

Addressables 지원을 활성화하는 방법:
1. 프로젝트에 `ADDRESSABLE_SUPPORT` 스크립팅 정의 심볼 추가
2. 도구가 자동으로 Addressable 경로를 생성된 상수에 포함

## 라이선스

이 프로젝트는 MIT 라이선스를 따릅니다 - 자세한 내용은 LICENSE 파일을 참조하세요.