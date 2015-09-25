![&mu; Mapper](docs/micro-mapper-logo.png?raw=true "&mu; Mapper")
================================

What is MicroMapper?
--------------------------------
MicroMapper is a spinoff project based on [AutoMapper]( http://github.com/AutoMapper/AutoMapper/). The problem of mapping is a deceptively complex one, whose aim of this library is the getting rid of code that mapped one object to another. This type of code is rather tedious and boring to write, which provides the motivation to solve the problem in a concice manner.

Additionally, MicroMapper makes permits extensible use of different mapper contexts. Let's say you have ``Controller A`` and ``Controller B`` each with its own set of mapping concerns, neither of which with shared mapping concerns.

```C#
public abstract class Controller
{
    IMapperContext Context { get; } = new MapperContext();
}

public class ControllerA : Controller { }

public class ControllerB : Controller { }
```

The above snippet illustrates how to initialize Context with a MapperContext specific to each controller. If dependency injection is a goal, then that can be accommodated as well.

```C#
public abstract class Controller
{
    IMapperContext Context { get; }

    protected Controller(IMapperContext mc)
    {
        Context = mc;
    }
}

public class ControllerA : Controller
{
    ControllerA(IMapperContext mc) : base(mc) { }
}

public class ControllerB : Controller
{
    ControllerB(IMapperContext mc) : base(mc) { }
}
```

How do I get started?
--------------------------------
Check out the [getting started guide](http://github.com/mwpowellhtx/MicroMapper/wiki/Getting-started). When you're done there, the [wiki](http://github.com/mwpowellhtx/MicroMapper/wiki) goes in to the nitty-gritty details. Finally, check out the [dnrTV episode](http://www.dnrtv.com/default.aspx?showNum=155) for a full walkthrough. The fundamentals are the same even though the specifics of whether to use static ```Mapper``` or instance ``IMapperContext`` are in a state of transition. If you have questions, you can post them to the [Mailing List](http://groups.google.com/group/micromapper-users).

Where can I get it?
--------------------------------
~~First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [MicroMapper](https://www.nuget.org/packages/MicroMapper/) from the package manager console:

```
PM> Install-Package MicroMapper
```~~

AutoMapper is Copyright &copy; 2008-2012 [Jimmy Bogard](http://jimmybogard.lostechies.com) and other contributors under the [MIT license](LICENSE.txt).
MicroMapper is Copyright &copy; 2015 Michael W. Powell and other contributors under the [MIT license](LICENSE.txt).
