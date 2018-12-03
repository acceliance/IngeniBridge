rem dont forget to rename ibdb file in the command line
cd bin\Release\netcoreapp2.1
dotnet IngeniBridge.GenerateFullInventory.dll --StorageAccessorAssembly="IngeniBridge.StorageAccessor.InMemory.dll" --IBDatabase=..\..\IngeniBridge.Sample.MyCompany\IngeniBridge.Samples.MyCompany\MasterAssetMyCompany_2018_09_13.ibdb --InventoryFile=InventoryFileIB.xlsx
pause
