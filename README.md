# JsonPropertiesSorter
It sorts the json data in mixed data types according to data priority.

# Prerequisites
Before you begin you must have .Net Standard 2.1 installed and configured properly for your computer. Please see [Download .NET SDKs](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)

# Run Json Properties Sorter
You can use Console Application under Release folder to run Json Properties Sorter.

```c#
Paste your json content below and end with an empty line:
{
   "FirstName":"Arthur",
   "LastName":"Bertrand",
   "Adrress":{
      "StreetName":"Gedempte Zalmhaven",
      "Number":"4K",
      "City":{
         "Name":"Rotterdam",
         "Country":"Netherlands"
      },
      "ZipCode":"3011 BT"
   },
   "Age":35,
   "Hobbies":[
      "Fishing",
      "Rowing"
   ]
}
```

```c#
Output:
{
  "FirstName": "Arthur",
  "LastName": "Bertrand",
  "Age": 35,
  "Adrress": {
    "StreetName": "Gedempte Zalmhaven",
    "Number": "4K",
    "ZipCode": "3011 BT",
    "City": {
      "Name": "Rotterdam",
      "Country": "Netherlands"
    }
  },
  "Hobbies": [
    "Fishing",
    "Rowing"
  ]
}

```
--------
```c#
Paste your json content below and end with an empty line:
{
   "menu":{
      "popup":{
         "menuitem":[
            {
               "value":"New",
               "onclick":"CreateDoc()"
            },
            {
               "value":"Open",
               "onclick":"OpenDoc()"
            },
            {
               "value":"Save",
               "onclick":"SaveDoc()"
            }
         ]
      },
      "id":"file",
      "value":"File"
   }
}

Output:
{
  "menu": {
    "id": "file",
    "value": "File",
    "popup": {
      "menuitem": [
        {
          "value": "New",
          "onclick": "CreateDoc()"
        },
        {
          "value": "Open",
          "onclick": "OpenDoc()"
        },
        {
          "value": "Save",
          "onclick": "SaveDoc()"
        }
      ]
    }
  }
}
```


# License
This project is licensed under the MIT License - see the LICENSE.md file for details
