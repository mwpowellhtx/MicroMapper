$framework = '4.5.1x86'

properties {
    $base_dir = resolve-path .
    $build_dir = "$base_dir\Build"
    $dist_dir = "$base_dir\Release"
    $source_dir = "$base_dir\src"
    $tools_dir = "$base_dir\tools"
    $test_dir = "$build_dir\Test"
    $result_dir = "$build_dir\Results"
    $lib_dir = "$base_dir\lib"
    $pkgVersion = if ($env:build_number -ne $NULL) { $env:build_number } else { '5.2.0' }
    $assemblyVersion = $pkgVersion -replace "\-.*$", ".0"
    $assemblyFileVersion = $pkgVersion -replace "-[^0-9]*", "."
    $global:config = "debug"
    $framework_dir = Get-FrameworkDirectory
}

task default -depends local
task local -depends compile, test
task lite -depends compile, test-lite
task full -depends local, dist
task ci -depends clean, release, commonAssemblyInfo, local, dist

task clean {
   delete_directory "$build_dir"
   delete_directory "$dist_dir"
}

task release {
    $global:config = "Release"
}

task compile -depends clean { 
    exec { dnu restore }
    exec { dnu build $source_dir\MicroMapper --configuration $config}
    exec { & $source_dir\.nuget\Nuget.exe restore $source_dir\MicroMapper.sln }
    exec { msbuild /t:Clean /t:Build /p:Configuration=$config /v:q /p:NoWarn=1591 /nologo $source_dir\MicroMapper.sln }
}

task commonAssemblyInfo {
    $commit = if ($env:BUILD_VCS_NUMBER -ne $NULL) { $env:BUILD_VCS_NUMBER } else { git log -1 --pretty=format:%H }
    create-commonAssemblyInfo "$commit" "$source_dir\CommonAssemblyInfo.cs"
}

task test {
    create_directory "$build_dir\results"
    exec { & $source_dir\packages\Fixie.1.0.0.3\lib\Net45\Fixie.Console.exe --xUnitXml $result_dir\MicroMapper.UnitTests.Net4.xml $source_dir/UnitTests/bin/NET4/$config/MicroMapper.UnitTests.Net4.dll }
    exec { & $tools_dir\statlight\statlight.exe -x $source_dir/UnitTests/bin/SL5/$config/MicroMapper.UnitTests.xap -d $source_dir/UnitTests/bin/SL5/$config/MicroMapper.UnitTests.SL5.dll --ReportOutputFile=$result_dir\MicroMapper.UnitTests.SL5.xml --ReportOutputFileType=NUnit }
    exec { & $source_dir\packages\xunit.runners.2.0.0-beta5-build2785\tools\xunit.console.x86.exe $source_dir/UnitTests/bin/WinRT/$config/MicroMapper.UnitTests.WinRT.dll -xml $result_dir\MicroMapper.UnitTests.WinRT.xml -parallel none }
    exec { & $source_dir\packages\xunit.runners.2.0.0-beta5-build2785\tools\xunit.console.x86.exe $source_dir/UnitTests/bin/WP8/$config/MicroMapper.UnitTests.WP8.dll -xml $result_dir\MicroMapper.UnitTests.WP8.xml -parallel none }
}

task test-lite {
    create_directory "$build_dir\results"
    exec { & $source_dir\packages\Fixie.1.0.0.3\lib\Net45\Fixie.Console.exe --xUnitXml $result_dir\MicroMapper.UnitTests.Net4.xml $source_dir/UnitTests/bin/NET4/$config/MicroMapper.UnitTests.Net4.dll }
}

task dist {
    create_directory $build_dir
    create_directory $dist_dir
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\net46" "$dist_dir\net46"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\net45" "$dist_dir\net45"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\net40" "$dist_dir\net40"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\portable-net45+win+wpa81+wp80+MonoAndroid10+Xamarin.iOS10+MonoTouch10" "$dist_dir\Portable"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\sl50" "$dist_dir\sl50"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\wp80" "$dist_dir\wp80"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\wpa81" "$dist_dir\wpa81"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\win" "$dist_dir\win"
    copy_files "$source_dir\MicroMapper.Android\bin\$config" "$dist_dir\MonoAndroid"
    copy_files "$source_dir\MicroMapper.iOS\bin\$config" "$dist_dir\MonoTouch"
    copy_files "$source_dir\MicroMapper.iOS10\bin\$config" "$dist_dir\Xamarin.iOS10"
    copy_files "$source_dir\artifacts\bin\MicroMapper\$config\dotnet" "$dist_dir\dotnet"
    create-nuspec "$pkgVersion" "MicroMapper.nuspec"
    exec { & $base_dir\RefGen.exe ".NETPlatform,Version=v5.0" "dotnet" "$base_dir\MicroMapper.nuspec" "$source_dir\MicroMapper\MicroMapper.xproj" "$dist_dir\dotnet\MicroMapper.dll" }
}

# -------------------------------------------------------------------------------------------------------------
# generalized functions 
# --------------------------------------------------------------------------------------------------------------
function Get-FrameworkDirectory()
{
    $([System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory().Replace("v2.0.50727", "v4.0.30319"))
}

function global:zip_directory($directory, $file)
{
    delete_file $file
    cd $directory
    exec { & "$tools_dir\7-zip\7za.exe" a $file *.* }
    cd $base_dir
}

function global:delete_directory($directory_name)
{
  rd $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:delete_file($file)
{
    if($file) {
        remove-item $file  -force  -ErrorAction SilentlyContinue | out-null} 
}

function global:create_directory($directory_name)
{
  mkdir $directory_name  -ErrorAction SilentlyContinue  | out-null
}

function global:copy_files($source, $destination, $exclude = @()) {
    create_directory $destination
    Get-ChildItem $source -Recurse -Exclude $exclude | Copy-Item -Destination {Join-Path $destination $_.FullName.Substring($source.length)} 
}

function global:create-commonAssemblyInfo($commit, $filename)
{
    $date = Get-Date
    "using System;
using System.Reflection;
using System.Runtime.InteropServices;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: CLSCompliant(true)]
[assembly: AssemblyVersionAttribute(""$assemblyVersion"")]
[assembly: AssemblyFileVersionAttribute(""$assemblyFileVersion"")]
// Initially adopted for support in the year 2015
[assembly: AssemblyCopyrightAttribute(""Copyright � Michael Powell " + $date.Year + """)]
[assembly: AssemblyProductAttribute(""MicroMapper"")]
[assembly: AssemblyTrademarkAttribute(""MicroMapper"")]
[assembly: AssemblyCompanyAttribute("""")]
[assembly: AssemblyConfigurationAttribute(""Release"")]
[assembly: AssemblyInformationalVersionAttribute(""$commit"")]"  | out-file $filename -encoding "ASCII"    
}

function global:create-nuspec($version, $fileName)
{
    "<?xml version=""1.0""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"">
  <metadata>
    <id>MicroMapper</id>
    <version>$version</version>
    <authors>Michael W. Powell</authors>
    <owners>Michael W. Powell</owners>
    <licenseUrl>http://github.com/mwpowellhtx/MicroMapper/blob/master/LICENSE.txt</licenseUrl>
    <projectUrl></projectUrl>
    <iconUrl>http://github.com/mwpowellhtx/MicroMapper/blob/master/docs/micro-mapper-logo.png</iconUrl>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <summary>A convention-based object-object mapper</summary>
    <description>A convention-based object-object mapper. MicroMapper uses a fluent configuration API to define an object-object mapping strategy. MicroMapper uses a convention-based matching algorithm to match up source to destination values. Currently, MicroMapper is geared towards model projection scenarios to flatten complex object models to DTOs and other simple objects, whose design is better suited for serialization, communication, messaging, or simply an anti-corruption layer between the domain and application layer. MicroMapper goes a step further and isolates one mapping context from another, and also makes it increasingly dependency injection friendly.</description>
    <frameworkAssemblies>
      <frameworkAssembly assemblyName=""System.Collections"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Runtime"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Linq"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Linq.Expressions"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Linq.Queryable"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Text.RegularExpressions"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Reflection"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Reflection.Extensions"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Diagnostics.Debug"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.ObjectModel"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Runtime.Extensions"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""System.Threading"" targetFramework="".NETPortable4.5-Profile259"" />
      <frameworkAssembly assemblyName=""mscorlib"" targetFramework="".NETFramework4.5"" />
      <frameworkAssembly assemblyName=""System"" targetFramework="".NETFramework4.5"" />
      <frameworkAssembly assemblyName=""System.Core"" targetFramework="".NETFramework4.5"" />
      <frameworkAssembly assemblyName=""Microsoft.CSharp"" targetFramework="".NETFramework4.5"" />
      <frameworkAssembly assemblyName=""mscorlib"" targetFramework="".NETFramework4.0"" />
      <frameworkAssembly assemblyName=""System"" targetFramework="".NETFramework4.0"" />
      <frameworkAssembly assemblyName=""System.Core"" targetFramework="".NETFramework4.0"" />
      <frameworkAssembly assemblyName=""Microsoft.CSharp"" targetFramework="".NETFramework4.0"" />
      <frameworkAssembly assemblyName=""System"" targetFramework=""Silverlight5.0"" />
      <frameworkAssembly assemblyName=""System.Core"" targetFramework=""Silverlight5.0"" />
      <frameworkAssembly assemblyName=""mscorlib"" targetFramework=""Silverlight5.0"" />
      <frameworkAssembly assemblyName=""mscorlib"" targetFramework=""WindowsPhone8.0"" />
      <frameworkAssembly assemblyName=""System"" targetFramework=""WindowsPhone8.0"" />
      <frameworkAssembly assemblyName=""System.Core"" targetFramework=""WindowsPhone8.0"" />
      <frameworkAssembly assemblyName=""System.Collections"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Runtime"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Linq"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Linq.Expressions"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Linq.Queryable"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Text.RegularExpressions"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Reflection"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Reflection.Extensions"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Diagnostics.Debug"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.ObjectModel"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Runtime.Extensions"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Threading"" targetFramework=""WindowsPhoneApp8.1"" />
      <frameworkAssembly assemblyName=""System.Collections"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Runtime"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Linq"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Linq.Expressions"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Linq.Queryable"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Text.RegularExpressions"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Reflection"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Reflection.Extensions"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Diagnostics.Debug"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.ObjectModel"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Runtime.Extensions"" targetFramework="".NETCore4.5"" />
      <frameworkAssembly assemblyName=""System.Threading"" targetFramework="".NETCore4.5"" />
    </frameworkAssemblies>
    <dependencies>
      <group targetFramework=""portable-net45+win+wpa81+wp80+MonoAndroid10+Xamarin.iOS10+MonoTouch10"">
      </group>
      <group targetFramework=""net46"">
      </group>
      <group targetFramework=""net45"">
      </group>
      <group targetFramework=""net40"">
        <dependency id=""Microsoft.Bcl"" version=""1.1.9"" />
      </group>
      <group targetFramework=""sl50"">
      </group>
      <group targetFramework=""wp80"">
      </group>
      <group targetFramework=""wpa81"">
      </group>
      <group targetFramework=""win"">
      </group>
      <group targetFramework=""MonoAndroid"">
      </group>
      <group targetFramework=""MonoTouch"">
      </group>
      <group targetFramework=""Xamarin.iOS10"">
      </group>
    </dependencies>
  </metadata>
  <files>
    <file src=""$dist_dir\Portable\MicroMapper.dll"" target=""lib\portable-net45+win+wpa81+wp80+MonoAndroid10+Xamarin.iOS10+MonoTouch10"" />
    <file src=""$dist_dir\Portable\MicroMapper.pdb"" target=""lib\portable-net45+win+wpa81+wp80+MonoAndroid10+Xamarin.iOS10+MonoTouch10"" />
    <file src=""$dist_dir\Portable\MicroMapper.xml"" target=""lib\portable-net45+win+wpa81+wp80+MonoAndroid10+Xamarin.iOS10+MonoTouch10"" />
    <file src=""$dist_dir\net46\MicroMapper.dll"" target=""lib\net46"" />
    <file src=""$dist_dir\net46\MicroMapper.pdb"" target=""lib\net46"" />
    <file src=""$dist_dir\net46\MicroMapper.xml"" target=""lib\net46"" />
    <file src=""$dist_dir\net45\MicroMapper.dll"" target=""lib\net45"" />
    <file src=""$dist_dir\net45\MicroMapper.pdb"" target=""lib\net45"" />
    <file src=""$dist_dir\net45\MicroMapper.xml"" target=""lib\net45"" />
    <file src=""$dist_dir\net40\MicroMapper.dll"" target=""lib\net40"" />
    <file src=""$dist_dir\net40\MicroMapper.pdb"" target=""lib\net40"" />
    <file src=""$dist_dir\net40\MicroMapper.xml"" target=""lib\net40"" />
    <file src=""$dist_dir\sl50\MicroMapper.dll"" target=""lib\sl50"" />
    <file src=""$dist_dir\sl50\MicroMapper.pdb"" target=""lib\sl50"" />
    <file src=""$dist_dir\sl50\MicroMapper.xml"" target=""lib\sl50"" />
    <file src=""$dist_dir\wp80\MicroMapper.dll"" target=""lib\wp80"" />
    <file src=""$dist_dir\wp80\MicroMapper.pdb"" target=""lib\wp80"" />
    <file src=""$dist_dir\wp80\MicroMapper.xml"" target=""lib\wp80"" />
    <file src=""$dist_dir\wpa81\MicroMapper.dll"" target=""lib\wpa81"" />
    <file src=""$dist_dir\wpa81\MicroMapper.pdb"" target=""lib\wpa81"" />
    <file src=""$dist_dir\wpa81\MicroMapper.xml"" target=""lib\wpa81"" />
    <file src=""$dist_dir\win\MicroMapper.dll"" target=""lib\win"" />
    <file src=""$dist_dir\win\MicroMapper.pdb"" target=""lib\win"" />
    <file src=""$dist_dir\win\MicroMapper.xml"" target=""lib\win"" />
    <file src=""$dist_dir\MonoAndroid\MicroMapper.dll"" target=""lib\MonoAndroid"" />
    <file src=""$dist_dir\MonoAndroid\MicroMapper.pdb"" target=""lib\MonoAndroid"" />
    <file src=""$dist_dir\MonoAndroid\MicroMapper.xml"" target=""lib\MonoAndroid"" />
    <file src=""$dist_dir\MonoTouch\MicroMapper.dll"" target=""lib\MonoTouch"" />
    <file src=""$dist_dir\MonoTouch\MicroMapper.pdb"" target=""lib\MonoTouch"" />
    <file src=""$dist_dir\MonoTouch\MicroMapper.xml"" target=""lib\MonoTouch"" />
    <file src=""$dist_dir\Xamarin.iOS10\MicroMapper.dll"" target=""lib\Xamarin.iOS10"" />
    <file src=""$dist_dir\Xamarin.iOS10\MicroMapper.pdb"" target=""lib\Xamarin.iOS10"" />
    <file src=""$dist_dir\Xamarin.iOS10\MicroMapper.xml"" target=""lib\Xamarin.iOS10"" />
    <file src=""$dist_dir\dotnet\MicroMapper.dll"" target=""lib\dotnet"" />
    <file src=""$dist_dir\dotnet\MicroMapper.pdb"" target=""lib\dotnet"" />
    <file src=""$dist_dir\dotnet\MicroMapper.xml"" target=""lib\dotnet"" />
    <file src=""src\**\*.cs"" target=""src"" />
  </files>
</package>" | out-file $fileName -encoding "ASCII"
}
