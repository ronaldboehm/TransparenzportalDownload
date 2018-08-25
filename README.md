# Transparenzportal Download

Console app to download data on Baugenehmigungen (building permits) from the [Transparenzportal Hamburg](http://transparenz.hamburg.de/) and to save relevant information as csv files. 

## Getting Started

The Transparenzportal Hamburg provides an [API](http://transparenz.hamburg.de/hinweise-zur-api/) to search and download public documents as required by the [Transparenzgesetz](http://www.hamburg.de/transparenzgesetz/). This application is used to download [Baugenehmigungen](http://www.hamburg.de/baugenehmigung/) from the portal, but could be used as a template to download other data.

### Prerequisites

The source code is for a [.NET Core 2.1](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial) application written in C#. To build and run it, you need the .NET Core SDK 2.1 that you can [download here](https://www.microsoft.com/net/download/dotnet-core/2.1).

To make changes to the source code, I would recommend using the [Visual Studio Code](https://code.visualstudio.com/) editor available for macOS, Windows, and Linux. On Windows, you can also download the Community edition of [Visual Studio](https://www.visualstudio.com/de/downloads/) instead. 

### Running the application

Open a command line and navigate to the directory containing the project file:

    D:\Dev\Git\TransparenzportalDownload\src\TransparenzportalDownload>dir *.csproj
    
    31.07.2018  11:38               280 TransparenzportalDownload.csproj
                   1 Datei(en),            280 Bytes
                   0 Verzeichnis(se), 357.660.602.368 Bytes frei

Now build the application using the .NET command line tool:

    D:\Dev\Git\TransparenzportalDownload\src\TransparenzportalDownload>dotnet build

The output should be similar to this:

    Microsoft (R)-Buildmodul, Version 15.7.179.6572 für .NET Core
    Copyright (C) Microsoft Corporation. Alle Rechte vorbehalten.

    [...]

    Der Buildvorgang wurde erfolgreich ausgeführt.
        0 Warnung(en)
        0 Fehler

    Verstrichene Zeit 00:00:00.76

If the build is successful, you can start the application:

    D:\Dev\Git\TransparenzportalDownload\src\TransparenzportalDownload>dotnet run

There will be console output:

    Getting Baugenehmigungen for tag "Baugenehmigung".
    Found 1000 more results.
    Found 1000 more results.
    [...]
    Found 724 more results.
    Found a total 25724 results for tag "Baugenehmigung".
    Writing to file D:\Dev\Git\TransparenzportalDownload\src\TransparenzportalDownload\baugenehmigungen.csv
    Done.

The downloaded data is saved to a file named *baugenehmigungen.csv* in the same directory.

## The Transparenzportal API

As explained on the [Transparenzportal webpage](http://transparenz.hamburg.de/hinweise-zur-api/), the portal builds on the [CKAN](http://docs.ckan.org/en/latest/api/index.html) platform, but even using this standard, accessing data depends on how the data is structured and stored in the *extras* dictionaries (see below).

### Downloading data

This simple app uses the CKAN *package search*. It first [retrieves possible tags](http://suche.transparenz.hamburg.de/api/3/action/tag_list) (in this case variations of the term *Baugenehmigung*), then calls [http://suche.transparenz.hamburg.de/api/3/action/package_search](http://suche.transparenz.hamburg.de/api/3/action/package_search?rows=10&start=1&fq=tags:Baugenehmigung) repeatedly. Downloaded data is first saved in the original format (in the *Downloads* subdirectory), then the relevant data is extracted and saved in *baugenehmigungen.csv*. 

### Data format

While each package (or dataset) contains the standard CKAN properties like *name*, *title*, and *author*, some of the relevant data is stored in the additional dictionaries named *extras* as key/value pairs. For example, a package may contain

```javascript
{ 
    "name": "errichtung-einer-unterkunft-zur-oeffentlich-rechtlichen-unterbringung-mit-102-plaetzen1",
    "extras": [
        {
            "key": "exact_publishing_date",
            "value": "2018-06-02T20:10:40"
        }
    ]
}
```

Of these, we currenty read exact_publishing_date, number, and file_reference_digital. Others can be added as needed.