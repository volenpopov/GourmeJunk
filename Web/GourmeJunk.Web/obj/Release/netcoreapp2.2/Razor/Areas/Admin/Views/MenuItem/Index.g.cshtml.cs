#pragma checksum "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f1d73a3ccfc2888debdcdd6fa7d4e73731b19fee"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_MenuItem_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/MenuItem/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Areas/Admin/Views/MenuItem/Index.cshtml", typeof(AspNetCore.Areas_Admin_Views_MenuItem_Index))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\_ViewImports.cshtml"
using GourmeJunk.Web;

#line default
#line hidden
#line 2 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\_ViewImports.cshtml"
using GourmeJunk.Web.Models;

#line default
#line hidden
#line 1 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
using GourmeJunk.Models.ViewModels.MenuItems;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f1d73a3ccfc2888debdcdd6fa7d4e73731b19fee", @"/Areas/Admin/Views/MenuItem/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"966d4390df66aff9690f17888bd09a63bc809817", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_MenuItem_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<MenuItemViewModel>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_CreateButtonPartial", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_TableButtonPartial", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(87, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 4 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
  
    ViewData["Title"] = "Index MenuItems";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
            BeginContext(187, 231, true);
            WriteLiteral("\r\n<br /><br />\r\n<div class=\"border backgroundWhite\">\r\n    <div class=\"row\">\r\n        <div class=\"col-6\">\r\n            <h2 class=\"text-info\"> Menu Items List</h2>\r\n        </div>\r\n        <div class=\"col-6 text-right\">\r\n            ");
            EndContext();
            BeginContext(418, 39, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "f1d73a3ccfc2888debdcdd6fa7d4e73731b19fee4757", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(457, 53, true);
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div>\r\n");
            EndContext();
#line 21 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
         if (Model.Count() > 0)
        {

#line default
#line hidden
            BeginContext(554, 172, true);
            WriteLiteral("            <table class=\"table table-responsive-sm table-striped border\">\r\n                <tr class=\"table-secondary\">\r\n                    <th>\r\n                        ");
            EndContext();
            BeginContext(727, 32, false);
#line 26 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayNameFor(m => m.Name));

#line default
#line hidden
            EndContext();
            BeginContext(759, 79, true);
            WriteLiteral("\r\n                    </th>\r\n                    <th>\r\n                        ");
            EndContext();
            BeginContext(839, 33, false);
#line 29 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayNameFor(m => m.Price));

#line default
#line hidden
            EndContext();
            BeginContext(872, 79, true);
            WriteLiteral("\r\n                    </th>\r\n                    <th>\r\n                        ");
            EndContext();
            BeginContext(952, 38, false);
#line 32 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayNameFor(m => m.CategoryId));

#line default
#line hidden
            EndContext();
            BeginContext(990, 79, true);
            WriteLiteral("\r\n                    </th>\r\n                    <th>\r\n                        ");
            EndContext();
            BeginContext(1070, 41, false);
#line 35 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayNameFor(m => m.SubCategoryId));

#line default
#line hidden
            EndContext();
            BeginContext(1111, 114, true);
            WriteLiteral("\r\n                    </th>\r\n                    <th></th>\r\n                    <th></th>\r\n                </tr>\r\n");
            EndContext();
#line 40 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                 foreach (var item in Model)
                {

#line default
#line hidden
            BeginContext(1290, 72, true);
            WriteLiteral("                <tr>\r\n                    <td>\r\n                        ");
            EndContext();
            BeginContext(1363, 31, false);
#line 44 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayFor(m => item.Name));

#line default
#line hidden
            EndContext();
            BeginContext(1394, 79, true);
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
            EndContext();
            BeginContext(1474, 32, false);
#line 47 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayFor(m => item.Price));

#line default
#line hidden
            EndContext();
            BeginContext(1506, 79, true);
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
            EndContext();
            BeginContext(1586, 39, false);
#line 50 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayFor(m => item.CategoryName));

#line default
#line hidden
            EndContext();
            BeginContext(1625, 79, true);
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
            EndContext();
            BeginContext(1705, 42, false);
#line 53 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                   Write(Html.DisplayFor(m => item.SubCategoryName));

#line default
#line hidden
            EndContext();
            BeginContext(1747, 79, true);
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
            EndContext();
            BeginContext(1826, 54, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "f1d73a3ccfc2888debdcdd6fa7d4e73731b19fee10661", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
#line 56 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Model = item.Id;

#line default
#line hidden
            __tagHelperExecutionContext.AddTagHelperAttribute("model", __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Model, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1880, 52, true);
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n");
            EndContext();
#line 59 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
                }

#line default
#line hidden
            BeginContext(1951, 22, true);
            WriteLiteral("            </table>\r\n");
            EndContext();
#line 61 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
        }
        else
        {

#line default
#line hidden
            BeginContext(2009, 38, true);
            WriteLiteral("            <p>No items exist...</p>\r\n");
            EndContext();
#line 65 "E:\Volen\Programming\Github\GourmeJunk\Web\GourmeJunk.Web\Areas\Admin\Views\MenuItem\Index.cshtml"
        }

#line default
#line hidden
            BeginContext(2058, 22, true);
            WriteLiteral("    </div>\r\n</div>\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<MenuItemViewModel>> Html { get; private set; }
    }
}
#pragma warning restore 1591
