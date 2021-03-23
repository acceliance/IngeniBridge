IngeniBridge Git Repo

This repo contains tools to help building and exploiting a local IngeniBridge database

tutorial folder : a tutorial to follow to build your own meta data

dataviz demo folder : a jquery ajax to integrate dataviz REST requests

Projects listed below:

IngeniBridge.Programs\IngeniBridge.GenerateFullInventory
	-	This project shows you how to parse the IngeniBridge database a make a dump to an Excel worksheet (uses EPPlus package) => build IngeniBridge.Sample.MyCompany to run it

IngeniBridge.Programs\IngeniBridge.IBDatabaseParser
	-	This project shows you how to parse the IngeniBridge database a make a full and deep dump to screen => build IngeniBridge.Sample.MyCompany to run it

IngeniBridge.Sample.MyCompany
	-	This project is a startup project for your metabase
	-	Includes
	-		Metamodel project => MyCompanyDataModel
	-			Contains the entire metamodel : entities, relations, inheritance and attributes
	-		Metadata project => IngeniBridge.Samples.MyCompany
	-			Contains data consolidation, check and serialization (produces an IBDB file to be mounted in the IngeniBridge Server)

IngeniBridge.TestServer
	-	This project is a demonstration on how to invoke an IngeniBridge Server
	-		REST only access
	-		Metamodel regeneration access
	-		Metamodel regeneration + Application Neutrality pattern (the application intelligently uses the metamodel to auto adapt to changes/evolutions on it)

IngeniBridge.Server/WebDeploy
	-	This folder contains the full Ingenibridge Server binaries ready to deploy in your on-premises IIS

IngeniBridge.Code_documentation
	- Documentation for the Core library