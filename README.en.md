# Auto Path Generator for Unity

A Unity editor tool that automatically generates constant string paths for Resources and Addressables (optional). This tool helps prevent hard-coded string paths in your Unity project and provides compile-time safety for resource paths.

## Features

- 🔄 Automatic path constant generation for Resources folder
- 🎯 Optional Addressables support
- ⚙️ Configurable output path for generated files
- 🔄 Auto-generation option on asset changes
- 🛡️ Compile-time safety for resource paths
- 🧹 Clean path name generation with special character handling

## Installation

### Via Package Manager

For Unity 2019.3.4f1 or higher, you can install the package directly through the Package Manager using a Git URL.

1. Open Package Manager (Window > Package Manager)
2. Click '+' button and select "Add package from git URL"
3. Enter the following URL:
```
https://github.com/jinhosung96/AutoPathGeneratorForUnity.git
```

Alternatively, you can add it directly to your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.jhs-library.auto-path-generator": "https://github.com/jinhosung96/AutoPathGeneratorForUnity.git"
  }
}
```

To install a specific version, add the #{version} tag to the URL:
```
https://github.com/jinhosung96/AutoPathGeneratorForUnity.git#1.0.0
```

## Usage

### Configuration

1. Access the settings through `Tools > Auto Path Generator > Settings`
2. Configure the following options:
   - **Output Path**: Directory where the generated path constants will be saved (relative to Assets folder)
   - **Auto Generate**: Toggle automatic generation on asset changes

### Manual Generation

You can manually generate path constants by:
1. Going to `Tools > Auto Path Generator > Generate`
2. Or selecting the config asset and clicking the "Generate" button

### Using Generated Paths

Generated paths are available in the `JHS.Library.AutoPathGenerator` namespace. Example usage:

```csharp
// Instead of using string literals
var prefab = Resources.Load("Prefabs/Characters/Player");

// Use the generated constants
var prefab = Resources.Load(ResourcesPaths.Prefabs_Characters_Player);
```

## Generated Code Structure

The tool generates a static class containing constant strings for all your resource paths:

```csharp
namespace JHS.Library.AutoPathGenerator
{
    public static class ResourcesPaths
    {
        public const string Example_Path = "Example/Path";
        // ... other paths
    }
    
    public static class AddressablePaths // If Addressables support is enabled
    {
        public const string Example_Address = "Example/Address";
        // ... other addresses
    }
}
```

## Special Path Handling

- Spaces are replaced with underscores
- Forward and backward slashes are converted to underscores
- Special characters are removed
- Numbers at the start of path names are prefixed with underscore
- C# keywords are prefixed with @ symbol
- Multiple consecutive underscores are consolidated

## Addressables Support

To enable Addressables support:
1. Add the `ADDRESSABLE_SUPPORT` scripting define symbol to your project
2. The tool will automatically include Addressable paths in the generated constants

## License

This project is licensed under the MIT License - see the LICENSE file for details.