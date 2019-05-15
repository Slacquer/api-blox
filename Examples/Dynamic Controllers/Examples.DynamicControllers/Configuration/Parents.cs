using System.Collections.Generic;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types;
using Examples.Resources;

namespace Examples.Configuration
{
    internal static class Parents
    {
        public static List<IComposedTemplate> AddParentsControllerTemplates(this List<IComposedTemplate> templates)
        {
            templates.WriteQueryByController<ParentRequest, ParentResponse>(new DynamicControllerTemplateOptions
                {
                    ActionRoute = "{parentId}",
                    ActionComments = new DynamicComments
                    {
                        Summary = "Get all parents action",
                        Remarks = @"
![tiny arrow](https://sourceforge.net/images/icon_linux.gif ""tiny arrow"")


| Tables        | Are           |  Cool |
| ------------- |:-------------:| -----:|
| col 3 is      | right-aligned | $1600 |
| col 2 is      | centered      |   $12 |
| zebra stripes | are neat      |    $1 |

*this is in italic*  and _so is this_

**this is in bold**  and __so is this__

***this is bold and italic***  and ___so is this___

<s>this is strike through text</s>

------------------

    Tabbed out gets a blockQuote.


* an asterisk starts an unordered list
* and this is another item in the list
+ or you can also use the + character
- or the - character


[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/YOUTUBE_VIDEO_ID_HERE/0.jpg)](http://www.youtube.com/watch?v=YOUTUBE_VIDEO_ID_HERE)
"
                    },
                    ControllerName = "Parents",
                    ControllerRoute = "api/[controller]/parents",
                    ControllerComments = new DynamicComments
                    {
                        Summary = "All necessary endpoints for Parent resource manipulation."
                    },
                    NameSpace = "Examples"
                }
            );

            return templates;
        }
    }
}
