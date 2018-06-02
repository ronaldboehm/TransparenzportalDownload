# Transparenzportal Download

Console app to download data on Baugenehmigungen (building permits) from the [Transparenzportal Hamburg](http://transparenz.hamburg.de/) and to save relevant information as csv files. 

## Getting Started

The Transparenzportal Hamburg provides an [API](http://transparenz.hamburg.de/hinweise-zur-api/) to search and download public documents as required by the [Transparenzgesetz](http://www.hamburg.de/transparenzgesetz/). This application is used to download [Baugenehmigungen](http://www.hamburg.de/baugenehmigung/) from the portal, but could be used as a template to download other data.

### Prerequisites

The source code is for a C# application. To run it, you can download the Community edition of [Visual Studio](https://www.visualstudio.com/de/downloads/). 

### Running the application

Open the .sln file, build and run the application in Visual Studio. The downloaded data is saved to a file named *baugenehmigungen.csv* in the *bin/Debug* subdirectory.

## The Transparenzportal API

As explained on the [Transparenzportal webpage](http://transparenz.hamburg.de/hinweise-zur-api/), the portal builds on the [CKAN](http://docs.ckan.org/en/latest/api/index.html) platform. Even using this standard, accessing data depends on how the data is structured and stored in the *extras* dictionaries (see below).

### Downloading data

This simple app uses the CKAN *package search*. It first [retrieves possible tags](http://suche.transparenz.hamburg.de/api/3/action/tag_list) (in this case variations of the term *Baugenehmigung*), then calls [http://suche.transparenz.hamburg.de/api/3/action/package_search](http://suche.transparenz.hamburg.de/api/3/action/package_search?rows=10&start=1&fq=tags:Baugenehmigung) repeatedly. Downloaded data is first saved in the original format (in the *bin\Debug\Downloads* subdirectory), then the relevant data is extracted and saved in *bin\Debug\baugenehmigungen.csv*). 

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