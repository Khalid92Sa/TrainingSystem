#pragma checksum "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d299fb69c0735cd1ed8132f9ef211cb2611806d4"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Section_InsertTrainees), @"mvc.1.0.view", @"/Views/Section/InsertTrainees.cshtml")]
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
#nullable restore
#line 1 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\_ViewImports.cshtml"
using TrainingSystem.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\_ViewImports.cshtml"
using TrainingSystem.Web.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d299fb69c0735cd1ed8132f9ef211cb2611806d4", @"/Views/Section/InsertTrainees.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4689150f329c4bcd82ab3df0da8b7999f3a6027e", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Section_InsertTrainees : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<TrainingSystem.Domain.Section>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "InsertTrainees", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
  
    ViewData["Title"] = "InsertTrainees";

#line default
#line hidden
#nullable disable
            WriteLiteral("<h2>Insert Trainees to Section</h2>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d299fb69c0735cd1ed8132f9ef211cb2611806d44396", async() => {
                WriteLiteral("\r\n    <div class=\"form-group\">\r\n        <label class=\"control-label\">Section ID: ");
#nullable restore
#line 8 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                            Write(Model.ID);

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n        <input");
                BeginWriteAttribute("value", " value=\"", 273, "\"", 290, 1);
#nullable restore
#line 9 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
WriteAttributeValue("", 281, Model.ID, 281, 9, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" type=\"hidden\" />\r\n    </div>\r\n    <div class=\"form-group\">\r\n        <label class=\"control-label\">Section Name: ");
#nullable restore
#line 12 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                              Write(Model.SectionField.SectionField);

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        <label class=\"control-label\">Trainer Name: ");
#nullable restore
#line 15 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                              Write(Model.Trainer.Name);

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        <label class=\"control-label\">Start Date: ");
#nullable restore
#line 18 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                            Write(Model.StartDate);

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n    </div>\r\n    <div class=\"form-group\">\r\n        <label class=\"control-label\">End Date: ");
#nullable restore
#line 21 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                          Write(Model.EndDate);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</label>
    </div>
    <div class=""form-group"">
        <div class=""col-md-offset-2 col-md-10"">
        </div>
    </div>
    <table class=""table mytable"" id=""mytable"">
        <thead>
            <tr>
                <th>
                    Select
                </th>
                <th>
                    TraineeID
                </th>
                <th>
                    Name
                </th>
                <th>
                    Status
                </th>
            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 45 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
              
                var trainees = ViewBag.Trainees;
                foreach (var trainee in trainees)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <td>\r\n                            <input type=\"checkbox\"\r\n                               name=\"selectedTrainees\"");
                BeginWriteAttribute("value", "\r\n                               value=\"", 1644, "\"", 1702, 1);
#nullable restore
#line 53 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
WriteAttributeValue("", 1684, trainee.TraineeID, 1684, 18, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("\r\n                               ");
#nullable restore
#line 54 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                           Write(Html.Raw(trainee.Assigned ? "checked=\"checked\"" : ""));

#line default
#line hidden
#nullable disable
                WriteLiteral(" />\r\n                        </td>\r\n                        <td>\r\n");
#nullable restore
#line 57 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                             if (@trainee.TraineeID < 10)
                            {
                                string x = "TE-0" + @trainee.TraineeID;
                                

#line default
#line hidden
#nullable disable
                WriteLiteral("<p>");
#nullable restore
#line 60 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                              Write(x);

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n");
#nullable restore
#line 61 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <p>TE-");
#nullable restore
#line 64 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                 Write(trainee.TraineeID);

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n");
#nullable restore
#line 65 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                            }

#line default
#line hidden
#nullable disable
                WriteLiteral("                        </td>\r\n                        <td>\r\n                            ");
#nullable restore
#line 68 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                       Write(trainee.Name);

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                        </td>\r\n                        <td>\r\n");
#nullable restore
#line 71 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                             if (@trainee.IsInOtherSection == true)
                            {
                                if (@trainee.Assigned == true)
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <p>Within this section</p>\r\n");
#nullable restore
#line 76 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                }
                                else
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <p>Within another section</p>\r\n");
#nullable restore
#line 80 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                                }

                            }
                            else
                            {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <p>empty</p>\r\n");
#nullable restore
#line 86 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                            }

#line default
#line hidden
#nullable disable
                WriteLiteral("                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 89 "C:\Users\one_touch\source\repos\TrainingSystem\TrainingSystem.Web\Views\Section\InsertTrainees.cshtml"
                }

            

#line default
#line hidden
#nullable disable
                WriteLiteral("        </tbody>\r\n    </table>\r\n    <div class=\"form-group\">\r\n        <input type=\"submit\" value=\"Save\" class=\"btn btn-primary\" />\r\n    </div>\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n<div>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d299fb69c0735cd1ed8132f9ef211cb2611806d414198", async() => {
                WriteLiteral("Back To Sections");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</div>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<TrainingSystem.Domain.Section> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
