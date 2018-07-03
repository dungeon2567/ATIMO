Fonte 1: http://forloop.co.uk/blog/managing-scripts-for-razor-partial-views-and-templates-in-asp.net-mvc
Fonte 2: https://bitbucket.org/forloop/forloop-htmlhelpers/wiki/Home


HtmlHelpers for managing scripts for Razor Partial Views and Templates in ASP.NET MVC
Available on NUGET

License Details
// Copyright (c) 2013 forloop - Russ Cam
// http://forloop.co.uk
// -------------------------------------------------------
// Dual licensed under the MIT and GPL licenses.
//   - http://www.opensource.org/licenses/mit-license
//   - http://www.opensource.org/licenses/gpl-3.0
What they are for
Managing JavaScript files and blocks of code can be tricky in ASP.NET MVC. Here are a few HTML helpers to smooth the situation.

Let's imagine you have a partial view that requires the addition of a script file to work and also needs to write out some script to wire up functionality for the partial. Let's also imagine that the script file required for the partial is dependent on another script file that is included at the bottom of the layout such that the <script> tag used to reference the file should be written into the response after the file it is dependent on. Using these helpers solves the problem of ensuring that the scripts are written out in the correct order.

How to use
Add a call to Html.RenderScripts()
First things first, you need to add a call to Html.RenderScripts() somewhere in your layout so that all of the script file references and blocks added using the helpers during the rendering of a Razor view are outputted in the response. The typical place to put this call is after the core scripts in your top level layout. Here's an example layout:

<!DOCTYPE html>
<html lang="en"> 
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
        <title>@ViewBag.Title</title>
        <meta name="viewport" content="width=device-width">
    </head>
    <body>       
        @RenderBody()           
        <script src="~/Scripts/jquery-2.0.2.js"></script>
        @* Call just after the core scripts in the top level layout *@
        @Html.RenderScripts()    
    </body>
</html>
If you're using the Microsoft ASP.NET Web Optimization framework, you can use the overload of Html.RenderScripts() to use the Scripts.Render() method as the function for generating scripts:

@Html.RenderScripts(Scripts.Render)
Script contexts
A script context needs to be used for writing scripts. You can create and use one in one of three ways:

1.Using the Html.BeginScriptContext() and Html.EndScriptContext() methods

@{
    // begin a context for the scripts
    Html.BeginScriptContext();

    Html.AddScriptBlock(@"$(function() { alert('hello from the page'); } });");
    Html.AddScriptFile("~/Scripts/jquery-ui-1.8.11.js");

    // end the script context
    Html.EndScriptContext();
}
2.Using a using statement and Html.AddScriptFile() and Html.AddScriptBlock() extension methods

// create a context for the scripts with a using statement
@using (Html.BeginScriptContext())
{
  Html.AddScriptFile("~/Scripts/jquery.validate.js");
  Html.AddScriptFile("~/Scripts/jquery-ui-1.8.11.js");
  Html.AddScriptBlock(@"$(function() { $('#someField').datepicker(); });");
}
3.Using a using statement and using the ScriptContext directly

// create a context for the scripts with a using statement
// and use the context directly
@using (var context = Html.BeginScriptContext())
{
  context.ScriptFiles.Add("~/Scripts/jquery-1.5.1.min.js");
  context.ScriptFiles.Add("~/Scripts/modernizr-1.7.min.js");
}  
Adding script files
Use the Html.AddScriptFile() or context.ScriptFiles.Add() methods to add a reference to a script file:

@using (Html.BeginScriptContext())
{
  Html.AddScriptFile("~/Scripts/jquery.validate.js");
}
or

@using (var context = Html.BeginScriptContext())
{
  context.ScriptFiles.Add("~/Scripts/jquery.validate.js");
}  
If you reference a script more than once, the helpers will ensure that it is rendered only once and in the ordinal position that matches the expected rendering order of views i.e.

Layout
Partials and Templates (in the order in which they appear in the view, top to bottom)
Adding script blocks
Use the Html.AddScriptBlock() method to add blocks of script. There are two overloads for the method; one that takes a string to render and one that uses a Razor template delegate to render the script:

1.Method overload that takes a string

@using (Html.BeginScriptContext())
{
  Html.AddScriptBlock(@"$(function() { $('#someField').datepicker(); });");
}
This method is primarily included for backwards compatibility. script tags should not be included as part of the string.

It is recommended to use method overload that uses a razor template delegate.

2.Method overload that takes a razor template delegate

@using (Html.BeginScriptContext())
{
  Html.AddScriptBlock(
    @<script type="text/javascript">
       $(function() { $('#someField').datepicker(); });
     </script>
  );
}
using this method overload provides you with full JavaScript intellisense in razor views and easily allows you to reference c# properties eithin the view, such as members of the ViewModel. Script tags must be included as part of the string (primarily for JavaScript intellisense and to notify the Razor parser that the block inside the Html.AddScriptBlock() method call is content).