using JsonParser.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Tests
{
    public class AttributeFirstJsonTransformerTests
    {
        private static bool CompareJson(string json1, string json2)
        {
            JToken token1 = JToken.Parse(json1);
            JToken token2 = JToken.Parse(json2);

            return JToken.DeepEquals(token1, token2);
        }
        
        private static void RunTest(string json, string expectedJson, string expectederrorMessage = null)
        {
            // Arrange
            var input = new MemoryStream(Encoding.ASCII.GetBytes(json));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            bool isTrue = expectederrorMessage == null
                ? CompareJson(actual, expectedJson)
                : actual.CompareTo(expectederrorMessage) == 0;

            Assert.AreEqual(isTrue, true);
        }

        [Test]
        public void WhenEmptyObjectThenEmptyObject()
        {
            string json = "{}";
            string expectedJson = "{}";
            RunTest(json, expectedJson);
        }

        [Test]
        public void WhenUnorderedPropertiesThenOrderedProperties()
        {
            string json = @"{
                              ""FirstName"": ""Arthur"",
                              ""LastName"": ""Bertrand"",
                              ""Adrress"": {
                                ""StreetName"": ""Gedempte Zalmhaven"",
                                ""Number"": ""4K"",
                                ""City"": {
                                  ""Name"": ""Rotterdam"",
                                  ""Country"": ""Netherlands""
                                },
                                ""ZipCode"": ""3011 BT""
                              },
                              ""Age"": 35,
                              ""Hobbies"": [
                                ""Fishing"",
                                ""Rowing""
                              ]
                            }";
            string expectedJson = @"{
                                     ""FirstName"": ""Arthur"",
                                     ""LastName"": ""Bertrand"",
                                     ""Age"": 35,
                                     ""Adrress"": {
                                       ""StreetName"": ""Gedempte Zalmhaven"",
                                       ""Number"": ""4K"",
                                       ""ZipCode"": ""3011 BT"",
                                       ""City"": {
                                         ""Name"": ""Rotterdam"",
                                         ""Country"": ""Netherlands""
                                       }
                                     },
                                     ""Hobbies"": [
                                       ""Fishing"",
                                       ""Rowing""
                                     ]
                                   }";

            RunTest(json, expectedJson);
        }

        [Test]
        public void NotValidJson()
        {
            string json = "edempte Zalmhaven\",\r\n      \"Number\":\"4K\",\r\n      \"City\":{\r\n         \"Name\":\"Rotterdam\",\r\n         \"Country\":\"Netherlands\"\r\n      },\r\n      \"ZipCode\":\"3011 BT\"\r\n   },\r\n   \"Age\":35,\r\n   \"Hobbies\":[\r\n      \"Fishing\",\r\n      \"Rowing\"\r\n   ]\r\n}";
            RunTest(json:json,expectedJson:null,expectederrorMessage: "It is not valid Json!");
        }

        [Test]
        public void WhenArrayJsonThenErrorMessage()
        {
            string json = "[ { color: \"red\", value: \"#f00\" }, { color: \"green\", value: \"#0f0\" },]";
            RunTest(json: json, expectedJson: null, expectederrorMessage: "It is not valid Json!");
        }

        [Test]
        public void WhenMixUnOrderedJsonThenOrderedJson()
        {
            string json = @"{
	                        ""batters"":
	                        	{
	                        		""batter"":
	                        			[
	                        				{ ""id"": ""1001"", ""type"": ""Regular"" },
	                        				{ ""id"": ""1002"", ""type"": ""Chocolate"" },
	                        				{ ""id"": ""1003"", ""type"": ""Blueberry"" },
	                        				{ ""id"": ""1004"", ""type"": ""Devil's Food"" }
	                        			]
	                        	},
	                        ""topping"":
	                        	[
	                        		{ ""id"": ""5001"", ""type"": ""None"" },
	                        		{ ""id"": ""5002"", ""type"": ""Glazed"" },
	                        		{ ""id"": ""5005"", ""type"": ""Sugar"" },
	                        		{ ""id"": ""5007"", ""type"": ""Powdered Sugar"" },
	                        		{ ""id"": ""5006"", ""type"": ""Chocolate with Sprinkles"" },
	                        		{ ""id"": ""5003"", ""type"": ""Chocolate"" },
	                        		{ ""id"": ""5004"", ""type"": ""Maple"" }
	                        	],
	                        ""id"": ""0001"",
	                        ""type"": ""donut"",
	                        ""name"": ""Cake"",
	                        ""ppu"": 0.55
                            }";
            string expectedJson = @"{
                            ""id"": ""0001"",
                            ""type"": ""donut"",
                            ""name"": ""Cake"",
                            ""ppu"": 0.55,
                            ""batters"": {
                              ""batter"": [
                                {
                                  ""id"": ""1001"",
                                  ""type"": ""Regular""
                                },
                                {
                                  ""id"": ""1002"",
                                  ""type"": ""Chocolate""
                                },
                                {
                                  ""id"": ""1003"",
                                  ""type"": ""Blueberry""
                                },
                                {
                                  ""id"": ""1004"",
                                  ""type"": ""Devil's Food""
                                }
                              ]
                            },
                            ""topping"": [
                              {
                                ""id"": ""5001"",
                                ""type"": ""None""
                              },
                              {
                                ""id"": ""5002"",
                                ""type"": ""Glazed""
                              },
                              {
                                ""id"": ""5005"",
                                ""type"": ""Sugar""
                              },
                              {
                                ""id"": ""5007"",
                                ""type"": ""Powdered Sugar""
                              },
                              {
                                ""id"": ""5006"",
                                ""type"": ""Chocolate with Sprinkles""
                              },
                              {
                                ""id"": ""5003"",
                                ""type"": ""Chocolate""
                              },
                              {
                                ""id"": ""5004"",
                                ""type"": ""Maple""
                              }
                            ]
                          }";
            RunTest(json, expectedJson);
        }

        [Test]
        public void WhenMixJsonWithNestedObjectAndArrayThenOrderedJson()
        {
            string json = @"{
	                ""items"":
	                	{
	                		""item"":
	                			[
	                				{
	                					""batters"":
	                						{
	                							""batter"":
	                								[
	                									{ ""id"": ""1001"", ""type"": ""Regular"" },
	                									{ ""id"": ""1002"", ""type"": ""Chocolate"" },
	                									{ ""id"": ""1003"", ""type"": ""Blueberry"" },
	                									{ ""id"": ""1004"", ""type"": ""Devil's Food"" }
	                								]
	                						},
	                					""topping"":
	                						[
	                							{ ""id"": ""5001"", ""type"": ""None"" },
	                							{ ""id"": ""5002"", ""type"": ""Glazed"" },
	                							{ ""id"": ""5005"", ""type"": ""Sugar"" },
	                							{ ""id"": ""5007"", ""type"": ""Powdered Sugar"" },
	                							{ ""id"": ""5006"", ""type"": ""Chocolate with Sprinkles"" },
	                							{ ""id"": ""5003"", ""type"": ""Chocolate"" },
	                							{ ""id"": ""5004"", ""type"": ""Maple"" }
	                						],
	                					""id"": ""0001"",
	                					""type"": ""donut"",
	                					""name"": ""Cake"",
	                					""ppu"": 0.55
	                				},
	                			]
	                	}
                    }";
            string expectedJson = @"{
                                      ""items"": {
                                        ""item"": [
                                          {
                                            ""id"": ""0001"",
                                            ""type"": ""donut"",
                                            ""name"": ""Cake"",
                                            ""ppu"": 0.55,
                                            ""batters"": {
                                              ""batter"": [
                                                {
                                                  ""id"": ""1001"",
                                                  ""type"": ""Regular""
                                                },
                                                {
                                                  ""id"": ""1002"",
                                                  ""type"": ""Chocolate""
                                                },
                                                {
                                                  ""id"": ""1003"",
                                                  ""type"": ""Blueberry""
                                                },
                                                {
                                                  ""id"": ""1004"",
                                                  ""type"": ""Devil's Food""
                                                }
                                              ]
                                            },
                                            ""topping"": [
                                              {
                                                ""id"": ""5001"",
                                                ""type"": ""None""
                                              },
                                              {
                                                ""id"": ""5002"",
                                                ""type"": ""Glazed""
                                              },
                                              {
                                                ""id"": ""5005"",
                                                ""type"": ""Sugar""
                                              },
                                              {
                                                ""id"": ""5007"",
                                                ""type"": ""Powdered Sugar""
                                              },
                                              {
                                                ""id"": ""5006"",
                                                ""type"": ""Chocolate with Sprinkles""
                                              },
                                              {
                                                ""id"": ""5003"",
                                                ""type"": ""Chocolate""
                                              },
                                              {
                                                ""id"": ""5004"",
                                                ""type"": ""Maple""
                                              }
                                            ]
                                          }
                                        ]
                                      }
                                    }";
            RunTest(json, expectedJson);
        }

        [Test]
        public void WhenObjectJsonWithArrayThenOrderedJson()
        {
            string json = @"{
                        	""items"":
                        		{
                        			""item"":
                        				[
                        					{
                        						""ppu"": 0,
                        						""type"": ""donut"",
                        						""name"": ""Cake"",
                        						""id"": ""0001""
                        					},
                        				]
                        		}
                        }";
            string expectedJson = @"{
                                  ""items"": {
                                    ""item"": [
                                      {
                                        ""type"": ""donut"",
                                        ""name"": ""Cake"",
                                        ""id"": ""0001"",
                                        ""ppu"": 0
                                      }
                                    ]
                                  }
                                }";
            RunTest(json, expectedJson);
        }

        [Test]
        public void WhenJsonWithNestedObjectsAndArraysThenOrderedJson()
        {
            string json = @"{
                        	""items"":
                        		{
                        			""item"":
                        				[
                        					{
                        						""topping"":
                        							[
                        								{ ""id"": ""5001"", ""type"": ""None"" },
                        								{ ""id"": ""5002"", ""type"": ""Glazed"" },
                        								{ ""id"": ""5005"", ""type"": ""Sugar"" },
                        								{ ""id"": ""5007"", ""type"": ""Powdered Sugar"" },
                        								{ ""id"": ""5006"", ""type"": ""Chocolate with Sprinkles"" },
                        								{ ""id"": ""5003"", ""type"": ""Chocolate"" },
                        								{ ""id"": ""5004"", ""type"": ""Maple"" }
                        							],
                        					},
                        				]
                        		}
                            }";
            string expectedJson = @"{
                                  ""items"": {
                                    ""item"": [
                                      {
                                        ""topping"": [
                                          {
                                            ""id"": ""5001"",
                                            ""type"": ""None""
                                          },
                                          {
                                            ""id"": ""5002"",
                                            ""type"": ""Glazed""
                                          },
                                          {
                                            ""id"": ""5005"",
                                            ""type"": ""Sugar""
                                          },
                                          {
                                            ""id"": ""5007"",
                                            ""type"": ""Powdered Sugar""
                                          },
                                          {
                                            ""id"": ""5006"",
                                            ""type"": ""Chocolate with Sprinkles""
                                          },
                                          {
                                            ""id"": ""5003"",
                                            ""type"": ""Chocolate""
                                          },
                                          {
                                            ""id"": ""5004"",
                                            ""type"": ""Maple""
                                          }
                                        ]
                                      }
                                    ]
                                  }
                                }";
            RunTest(json, expectedJson);
        }

        [Test]
        public void WhenJsonWithStringsAndNestedObjectsThenOrderedJson()
        {
            string json = @"{
                             ""glossary"":{
                                ""title"":""example glossary"",
                                ""GlossDiv"":{
                                   ""GlossList"":{
                                      ""GlossEntry"":{
                                         ""ID"":""SGML"",
                                         ""SortAs"":""SGML"",
                                         ""GlossTerm"":""Standard Generalized Markup Language"",
                                         ""Acronym"":""SGML"",
                                         ""Abbrev"":""ISO 8879:1986"",
                                         ""GlossDef"":{
                                            ""para"":""A meta-markup language, used to create markup languages such as DocBook."",
                                            ""GlossSeeAlso"":[
                                               ""GML"",
                                               ""XML""
                                            ]
                                         },
                                         ""GlossSee"":""markup""
                                      }
                                   },
                                   ""title"":""S"",
                                }
                             }
                          }";
            string expectedJson = @"{
                                     ""glossary"": {
                                       ""title"": ""example glossary"",
                                       ""GlossDiv"": {
                                         ""title"": ""S"",
                                         ""GlossList"": {
                                           ""GlossEntry"": {
                                             ""ID"": ""SGML"",
                                             ""SortAs"": ""SGML"",
                                             ""GlossTerm"": ""Standard Generalized Markup Language"",
                                             ""Acronym"": ""SGML"",
                                             ""Abbrev"": ""ISO 8879:1986"",
                                             ""GlossSee"": ""markup"",
                                             ""GlossDef"": {
                                               ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
                                               ""GlossSeeAlso"": [
                                                 ""GML"",
                                                 ""XML""
                                               ]
                                             }
                                           }
                                         }
                                       }
                                     }
                                   }";
            RunTest(json, expectedJson);
        }

        [Test]
        public void WhenMixUnOrderedJsonTooLongThenOrderedJson()
        {
            string json = @"{
                       ""web-app"":{
                          ""servlet"":[
                             {
                                ""servlet-name"":""cofaxCDS"",
                                ""servlet-class"":""org.cofax.cds.CDSServlet"",
                                ""init-param"":{
                                   ""configGlossary:installationAt"":""Philadelphia, PA"",
                                   ""configGlossary:adminEmail"":""ksm@pobox.com"",
                                   ""configGlossary:poweredBy"":""Cofax"",
                                   ""configGlossary:poweredByIcon"":""/images/cofax.gif"",
                                   ""configGlossary:staticPath"":""/content/static"",
                                   ""templateProcessorClass"":""org.cofax.WysiwygTemplate"",
                                   ""templateLoaderClass"":""org.cofax.FilesTemplateLoader"",
                                   ""templatePath"":""templates"",
                                   ""templateOverridePath"":"""",
                                   ""defaultListTemplate"":""listTemplate.htm"",
                                   ""defaultFileTemplate"":""articleTemplate.htm"",
                                   ""useJSP"":false,
                                   ""jspListTemplate"":""listTemplate.jsp"",
                                   ""jspFileTemplate"":""articleTemplate.jsp"",
                                   ""cachePackageTagsTrack"":200,
                                   ""cachePackageTagsStore"":200,
                                   ""cachePackageTagsRefresh"":60,
                                   ""cacheTemplatesTrack"":100,
                                   ""cacheTemplatesStore"":50,
                                   ""cacheTemplatesRefresh"":15,
                                   ""cachePagesTrack"":200,
                                   ""cachePagesStore"":100,
                                   ""cachePagesRefresh"":10,
                                   ""cachePagesDirtyRead"":10,
                                   ""searchEngineListTemplate"":""forSearchEnginesList.htm"",
                                   ""searchEngineFileTemplate"":""forSearchEngines.htm"",
                                   ""searchEngineRobotsDb"":""WEB-INF/robots.db"",
                                   ""useDataStore"":true,
                                   ""dataStoreClass"":""org.cofax.SqlDataStore"",
                                   ""redirectionClass"":""org.cofax.SqlRedirection"",
                                   ""dataStoreName"":""cofax"",
                                   ""dataStoreDriver"":""com.microsoft.jdbc.sqlserver.SQLServerDriver"",
                                   ""dataStoreUrl"":""jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon"",
                                   ""dataStoreUser"":""sa"",
                                   ""dataStorePassword"":""dataStoreTestQuery"",
                                   ""dataStoreTestQuery"":""SET NOCOUNT ON;select test='test';"",
                                   ""dataStoreLogFile"":""/usr/local/tomcat/logs/datastore.log"",
                                   ""dataStoreInitConns"":10,
                                   ""dataStoreMaxConns"":100,
                                   ""dataStoreConnUsageLimit"":100,
                                   ""dataStoreLogLevel"":""debug"",
                                   ""maxUrlLength"":500
                                }
                             },
                             {
                                ""servlet-name"":""cofaxEmail"",
                                ""servlet-class"":""org.cofax.cds.EmailServlet"",
                                ""init-param"":{
                                   ""mailHost"":""mail1"",
                                   ""mailHostOverride"":""mail2""
                                }
                             },
                             {
                                ""servlet-name"":""cofaxAdmin"",
                                ""servlet-class"":""org.cofax.cds.AdminServlet""
                             },
                             {
                                ""servlet-name"":""fileServlet"",
                                ""servlet-class"":""org.cofax.cds.FileServlet""
                             },
                             {
                                ""servlet-name"":""cofaxTools"",
                                ""servlet-class"":""org.cofax.cms.CofaxToolsServlet"",
                                ""init-param"":{
                                   ""templatePath"":""toolstemplates/"",
                                   ""log"":1,
                                   ""logLocation"":""/usr/local/tomcat/logs/CofaxTools.log"",
                                   ""logMaxSize"":"""",
                                   ""dataLog"":1,
                                   ""dataLogLocation"":""/usr/local/tomcat/logs/dataLog.log"",
                                   ""dataLogMaxSize"":"""",
                                   ""removePageCache"":""/content/admin/remove?cache=pages&id="",
                                   ""removeTemplateCache"":""/content/admin/remove?cache=templates&id="",
                                   ""fileTransferFolder"":""/usr/local/tomcat/webapps/content/fileTransferFolder"",
                                   ""lookInContext"":1,
                                   ""adminGroupID"":4,
                                   ""betaServer"":true
                                }
                             }
                          ],
                          ""servlet-mapping"":{
                             ""cofaxCDS"":""/"",
                             ""cofaxEmail"":""/cofaxutil/aemail/*"",
                             ""cofaxAdmin"":""/admin/*"",
                             ""fileServlet"":""/static/*"",
                             ""cofaxTools"":""/tools/*""
                          },
                          ""taglib"":{
                             ""taglib-uri"":""cofax.tld"",
                             ""taglib-location"":""/WEB-INF/tlds/cofax.tld""
                          }
                       }
                    }";
            string expectedJson = @"{
                                  ""web-app"": {
                                    ""servlet-mapping"": {
                                      ""cofaxCDS"": ""/"",
                                      ""cofaxEmail"": ""/cofaxutil/aemail/*"",
                                      ""cofaxAdmin"": ""/admin/*"",
                                      ""fileServlet"": ""/static/*"",
                                      ""cofaxTools"": ""/tools/*""
                                    },
                                    ""taglib"": {
                                      ""taglib-uri"": ""cofax.tld"",
                                      ""taglib-location"": ""/WEB-INF/tlds/cofax.tld""
                                    },
                                    ""servlet"": [
                                      {
                                        ""servlet-name"": ""cofaxCDS"",
                                        ""servlet-class"": ""org.cofax.cds.CDSServlet"",
                                        ""init-param"": {
                                          ""configGlossary:installationAt"": ""Philadelphia, PA"",
                                          ""configGlossary:adminEmail"": ""ksm@pobox.com"",
                                          ""configGlossary:poweredBy"": ""Cofax"",
                                          ""configGlossary:poweredByIcon"": ""/images/cofax.gif"",
                                          ""configGlossary:staticPath"": ""/content/static"",
                                          ""templateProcessorClass"": ""org.cofax.WysiwygTemplate"",
                                          ""templateLoaderClass"": ""org.cofax.FilesTemplateLoader"",
                                          ""templatePath"": ""templates"",
                                          ""templateOverridePath"": """",
                                          ""defaultListTemplate"": ""listTemplate.htm"",
                                          ""defaultFileTemplate"": ""articleTemplate.htm"",
                                          ""jspListTemplate"": ""listTemplate.jsp"",
                                          ""jspFileTemplate"": ""articleTemplate.jsp"",
                                          ""searchEngineListTemplate"": ""forSearchEnginesList.htm"",
                                          ""searchEngineFileTemplate"": ""forSearchEngines.htm"",
                                          ""searchEngineRobotsDb"": ""WEB-INF/robots.db"",
                                          ""dataStoreClass"": ""org.cofax.SqlDataStore"",
                                          ""redirectionClass"": ""org.cofax.SqlRedirection"",
                                          ""dataStoreName"": ""cofax"",
                                          ""dataStoreDriver"": ""com.microsoft.jdbc.sqlserver.SQLServerDriver"",
                                          ""dataStoreUrl"": ""jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon"",
                                          ""dataStoreUser"": ""sa"",
                                          ""dataStorePassword"": ""dataStoreTestQuery"",
                                          ""dataStoreTestQuery"": ""SET NOCOUNT ON;select test='test';"",
                                          ""dataStoreLogFile"": ""/usr/local/tomcat/logs/datastore.log"",
                                          ""dataStoreLogLevel"": ""debug"",
                                          ""cachePackageTagsTrack"": 200,
                                          ""cachePackageTagsStore"": 200,
                                          ""cachePackageTagsRefresh"": 60,
                                          ""cacheTemplatesTrack"": 100,
                                          ""cacheTemplatesStore"": 50,
                                          ""cacheTemplatesRefresh"": 15,
                                          ""cachePagesTrack"": 200,
                                          ""cachePagesStore"": 100,
                                          ""cachePagesRefresh"": 10,
                                          ""cachePagesDirtyRead"": 10,
                                          ""dataStoreInitConns"": 10,
                                          ""dataStoreMaxConns"": 100,
                                          ""dataStoreConnUsageLimit"": 100,
                                          ""maxUrlLength"": 500,
                                          ""useJSP"": false,
                                          ""useDataStore"": true
                                        }
                                      },
                                      {
                                        ""servlet-name"": ""cofaxEmail"",
                                        ""servlet-class"": ""org.cofax.cds.EmailServlet"",
                                        ""init-param"": {
                                          ""mailHost"": ""mail1"",
                                          ""mailHostOverride"": ""mail2""
                                        }
                                      },
                                      {
                                        ""servlet-name"": ""cofaxAdmin"",
                                        ""servlet-class"": ""org.cofax.cds.AdminServlet""
                                      },
                                      {
                                        ""servlet-name"": ""fileServlet"",
                                        ""servlet-class"": ""org.cofax.cds.FileServlet""
                                      },
                                      {
                                        ""servlet-name"": ""cofaxTools"",
                                        ""servlet-class"": ""org.cofax.cms.CofaxToolsServlet"",
                                        ""init-param"": {
                                          ""templatePath"": ""toolstemplates/"",
                                          ""logLocation"": ""/usr/local/tomcat/logs/CofaxTools.log"",
                                          ""logMaxSize"": """",
                                          ""dataLogLocation"": ""/usr/local/tomcat/logs/dataLog.log"",
                                          ""dataLogMaxSize"": """",
                                          ""removePageCache"": ""/content/admin/remove?cache=pages&id="",
                                          ""removeTemplateCache"": ""/content/admin/remove?cache=templates&id="",
                                          ""fileTransferFolder"": ""/usr/local/tomcat/webapps/content/fileTransferFolder"",
                                          ""log"": 1,
                                          ""dataLog"": 1,
                                          ""lookInContext"": 1,
                                          ""adminGroupID"": 4,
                                          ""betaServer"": true
                                        }
                                      }
                                    ]
                                  }
                                }";
            RunTest(json, expectedJson);
        }

        [Test]
        public void WhenMixUnOrderedJsonThenOrderedJson7()
        {
            string json = @"{
                            ""menu"": {
                            ""header"": ""SVG Viewer"",
                            ""items"": [
                                {""id"": ""Open""},
                                {""id"": ""OpenNew"", ""label"": ""Open New""},
                                {""id"": ""ZoomIn"", ""label"": ""Zoom In""},
                                {""id"": ""ZoomOut"", ""label"": ""Zoom Out""},
                                {""id"": ""OriginalView"", ""label"": ""Original View""},
                                {""id"": ""Quality""},
                                {""id"": ""Pause""},
                                {""id"": ""Mute""},
                                {""id"": ""Find"", ""label"": ""Find...""},
                                {""id"": ""FindAgain"", ""label"": ""Find Again""},
                                {""id"": ""Copy""},
                                {""id"": ""CopyAgain"", ""label"": ""Copy Again""},
                                {""id"": ""CopySVG"", ""label"": ""Copy SVG""},
                                {""id"": ""ViewSVG"", ""label"": ""View SVG""},
                                {""id"": ""ViewSource"", ""label"": ""View Source""},
                                {""id"": ""SaveAs"", ""label"": ""Save As""},
                                {""id"": ""Help""},
                                {""id"": ""About"", ""label"": ""About Adobe CVG Viewer...""}
                            ]
                        }}";
            string expectedJson = @"{
                              ""menu"": {
                                ""header"": ""SVG Viewer"",
                                ""items"": [
                                  {
                                    ""id"": ""Open""
                                  },
                                  {
                                    ""id"": ""OpenNew"",
                                    ""label"": ""Open New""
                                  },
                                  {
                                    ""id"": ""ZoomIn"",
                                    ""label"": ""Zoom In""
                                  },
                                  {
                                    ""id"": ""ZoomOut"",
                                    ""label"": ""Zoom Out""
                                  },
                                  {
                                    ""id"": ""OriginalView"",
                                    ""label"": ""Original View""
                                  },
                                  {
                                    ""id"": ""Quality""
                                  },
                                  {
                                    ""id"": ""Pause""
                                  },
                                  {
                                    ""id"": ""Mute""
                                  },
                                  {
                                    ""id"": ""Find"",
                                    ""label"": ""Find...""
                                  },
                                  {
                                    ""id"": ""FindAgain"",
                                    ""label"": ""Find Again""
                                  },
                                  {
                                    ""id"": ""Copy""
                                  },
                                  {
                                    ""id"": ""CopyAgain"",
                                    ""label"": ""Copy Again""
                                  },
                                  {
                                    ""id"": ""CopySVG"",
                                    ""label"": ""Copy SVG""
                                  },
                                  {
                                    ""id"": ""ViewSVG"",
                                    ""label"": ""View SVG""
                                  },
                                  {
                                    ""id"": ""ViewSource"",
                                    ""label"": ""View Source""
                                  },
                                  {
                                    ""id"": ""SaveAs"",
                                    ""label"": ""Save As""
                                  },
                                  {
                                    ""id"": ""Help""
                                  },
                                  {
                                    ""id"": ""About"",
                                    ""label"": ""About Adobe CVG Viewer...""
                                  }
                                ]
                              }
                            }";
            RunTest(json, expectedJson);
        }
    }
}