rem dont forget to rename ibdb file in the command line
cd bin\Release\netcoreapp2.1
dotnet.exe IngeniBridge.IBDatabaseParser.dll --StorageAccessorAssembly="IngeniBridge.StorageAccessor.InMemory.dll" --IBDatabase=..\..\..\..\..\IngeniBridge.Sample.MyCompany\MasterAssetMyCompany_2019_03_26.ibdb
pause