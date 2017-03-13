AzTable
=======

Demo project, produces single dll which then can be used via powershell to save data into azure tables

Usage example
-------------

```
Add-Type -Path .\AzTable.dll

$metrics = New-Object AzTable.Metrics('accountname', '**********==')
$metrics.Save('metric_category', 23, (Get-Date))
```


Producing single dll file
-------------------------

[ILmerge](https://www.nuget.org/packages/ilmerge/) is used to merge all assemblies into single one

Custom `AfterBuild` target is added for `Release` build

```
<Target Name="AfterBuild" Condition="'$(Configuration)' == 'Release'">
	<ConvertToAbsolutePath Paths="$(OutputPath)">
		<Output TaskParameter="AbsolutePaths" PropertyName="OutputFullPath" />
	</ConvertToAbsolutePath>
	<ItemGroup>
		<MergeAssemblies Include="$(OutputPath)$(MSBuildProjectName).dll" />
		<MergeAssemblies Include="$(OutputPath)System.Spatial.dll" />
		<MergeAssemblies Include="$(OutputPath)Microsoft.Data.Edm.dll" />
		<MergeAssemblies Include="$(OutputPath)Microsoft.Data.OData.dll" />
		<MergeAssemblies Include="$(OutputPath)Microsoft.Azure.KeyVault.Core.dll" />
		<MergeAssemblies Include="$(OutputPath)Microsoft.Data.Services.Client.dll" />
		<MergeAssemblies Include="$(OutputPath)Newtonsoft.Json.dll" />
		<MergeAssemblies Include="$(OutputPath)Microsoft.WindowsAzure.Storage.dll" />
	</ItemGroup>
	<PropertyGroup>
		<OutputAssembly>$(OutputFullPath)$(MSBuildProjectName).Standalone.dll</OutputAssembly>
		<Merger>$(SolutionDir)packages\ILMerge.2.14.1208\tools\ILMerge.exe</Merger>
	</PropertyGroup>
	<Message Text="Merge -&gt; $(OutputAssembly)" Importance="High" />
	<Exec Command="&quot;$(Merger)&quot; /target:dll /out:&quot;$(OutputAssembly)&quot; @(MergeAssemblies->'&quot;%(FullPath)&quot;', ' ')" />
	<Copy SourceFiles="$(OutputAssembly)" DestinationFiles="$(OutputPath)$(MSBuildProjectName).tmp" />
	<ItemGroup>
		<FilesToDelete Include="$(OutputPath)$(MSBuildProjectName).dll" />
		<FilesToDelete Include="$(OutputPath)*.pdb" />
		<FilesToDelete Include="$(OutputPath)*.xml" />
		<FilesToDelete Include="$(OutputPath)*.dll" />
		<FilesToDelete Include="$(OutputPath)*.config" />
	</ItemGroup>
	<Message Text="Cleanup -&gt; @(FilesToDelete, ', ')" Importance="High" />
	<Delete Files="@(FilesToDelete)" />
	<Copy SourceFiles="$(OutputPath)$(MSBuildProjectName).tmp" DestinationFiles="$(OutputPath)$(MSBuildProjectName).dll" />
	<Delete Files="$(OutputPath)$(MSBuildProjectName).tmp" />
	<RemoveDir Directories="$(OutputPath)de;$(OutputPath)es;$(OutputPath)fr;$(OutputPath)it;$(OutputPath)ja;$(OutputPath)ko;$(OutputPath)ru;$(OutputPath)zh-Hans;$(OutputPath)zh-Hant;" />
</Target>
```

Seems that `MergeAssemblies` should be declared in order from down to up, e.g. in my case the last one is `Microsoft.WindowsAzure.Storage` which depends on `Newtonsoft.Json`

Take a note that after ilmerge upgrade path to it should be upgraded manually

