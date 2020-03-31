using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Configuration.Install;
using System.Drawing.Printing;
using System.IO;
using System.ComponentModel;
using System.Management;
using System.Security.Policy;
using System.Web.Http;
using System.Reflection;
using System.Data.SqlClient;
using System.Runtime.Remoting;
using System.Security;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;



namespace Utils.Exceptions
{
    public class ApiExceptionAttribute
    {
         
        public static Dictionary< HttpStatusCode,string> HttpExParam(Exception ex)
        {
            Dictionary<HttpStatusCode, string> sList = new Dictionary<HttpStatusCode, string>();
            string res = "处理异常";
            HttpStatusCode extype = HttpStatusCode.OK;
            if (ex is NotImplementedException)
            {
                extype = HttpStatusCode.NotImplemented;
                res = "服务器不支持请求的函数";
            }
            else if (ex is TimeoutException)
            {
                extype =HttpStatusCode.RequestTimeout;
                res = "超时";
            }
            else if (ex is HttpResponseException)
            {
                extype = HttpStatusCode.NotFound;
                res = "没有找到对象";
            }
            else if (ex is UnauthorizedAccessException)
            {
                extype = HttpStatusCode.Unauthorized;
                res = "当操作系统因 I/O 错误或指定类型的安全错误而拒绝访问时所引发的异常";
            }
            else if (ex is DivideByZeroException)
            {
                extype =(HttpStatusCode)HttpStatusCodeMore.DivideByZero;
                res = "试图用零除整数值时引发";
            }
            else if (ex is AccessViolationException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.AccessViolation;
                res = "尝试读取或写入受保护的内存";
            }
            else if (ex is AggregateException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Aggregate;
                res = "聚合异常";
            }
            else if (ex is AppDomainUnloadedException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.AppDomainUnloaded;
                res = "在尝试访问已卸载的应用程序域时引发的异常";
            }
            else if (ex is ApplicationException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Application;
                res = "Application Exception";
            }
            else if (ex is ArgumentException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Argument;
                res = "在向方法提供的其中一个参数无效时引发的异常";
            }
            else if (ex is ArgumentNullException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.ArgumentNull;
                res = "值不能为 null";
            }
            else if (ex is ArithmeticException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Arithmetic;
                res = "因算术运算、类型转换或转换操作中的错误而引发的异常";
            }
            else if (ex is ArgumentOutOfRangeException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.ArgumentOutOfRange;
                res = "指定的参数已超出有效值的范围";
            }
            else if (ex is BadImageFormatException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.BadImageFormat;
                res = "当 DLL 或可执行程序的文件图像无效时引发的异常";
            }
            else if (ex is CannotUnloadAppDomainException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.CannotUnloadAppDomain;
                res = "卸载应用程序域的尝试失败时引发的异常";
            }
            else if (ex is ContextMarshalException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.ContextMarshal;
                res = "在尝试将对象封送过上下文边界失败时引发的异常";
            }
            else if (ex is CookieException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Cookie;
                res = "cookie异常";
            }
            else if (ex is DataMisalignedException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.DataMisaligned;
                res = "数据对齐异常";
            }
            else if (ex is DllNotFoundException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.DllNotFound;
                res = "无法加载DLL";
            }
            else if (ex is DuplicateWaitObjectException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.DuplicateWaitObject;
                res = "重复等待对象异常";
            }
            else if (ex is ConfigurationException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Configuration;
                res = "重复等待对象异常";
            }
            else if (ex is DataException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Data;
                res = "表示使用 ADO.NET 组件发生错误时引发的异常";
            }
            else if (ex is DBConcurrencyException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.DBConcurrency;
                res = "在更新操作过程中受影响的行数等于零时，由 DataAdapter 所引发的异常";
            }
            else if (ex is DuplicateNameException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.DuplicateName;
                res = "重复的名称例外";
            }
            else if (ex is EntryPointNotFoundException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.EntryPointNotFound;
                res = "未发现入口点";
            }
            else if (ex is ExecutionEngineException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.ExecutionEngine;
                res = "ExecutionEngineException";
            }
            else if (ex is FieldAccessException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.FieldAccess;
                res = "FieldAccessException";
            }
            else if (ex is FormatException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Format;
                res = "当参数格式不符合调用的方法的参数规范时引发的异常";
            }
            else if (ex is HttpCompileException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.HttpCompile;
                res = "HTTP编译异常";
            }
            else if (ex is HttpException || ex is HttpListenerException||ex is HttpParseException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Http;
                res = "HTTP异常";
            }
            else if (ex is HttpRequestValidationException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.HttpRequestValidation;
                res = "从客户端中检测到有潜在危险的 Request.Form 值";
            }
            else if (ex is HttpUnhandledException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.HttpUnhandled;
                res = "HttpUnhandledException";
            }
            else if (ex is InstallException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Install;
                res = "在安装的提交、回滚或卸载阶段发生错误时引发的异常";
            }
            else if (ex is InvalidPrinterException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.InvalidPrinter;
                res = "表示当试图用无效的打印机设置来访问打印机时所引发的异常";
            }
            else if (ex is InvalidOperationException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.InvalidOperation;
                res = "当方法调用对于对象的当前状态无效时引发的异常";
            }
            else if (ex is InternalBufferOverflowException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.InternalBufferOverflow;
                res = " 内部缓冲区溢出时引发的异常";
            }
            else if (ex is IOException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.IO;
                res = "发生 I/O 错误时引发的异常";
            }
            else if (ex is LicenseException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.License;
                res = " 表示当组件不能被授予许可证时引发的异常";
            }
            else if (ex is ManagementException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Management;
                res = "管理异常";
            }
            else if (ex is NotImplementedException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.NotImplemented;
                res = "在无法实现请求的方法或操作时引发的异常";
            }
            else if (ex is NotSupportedException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.NotSupported;
                res = "当调用的方法不受支持，或试图读取、查找或写入不支持调用功能的流时引发的异常";
            }
            else if (ex is PolicyException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Policy;
                res = "当策略禁止代码运行时引发的异常";
            }
            else if (ex is RankException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Rank;
                res = "将维数错误的数组传递给方法时引发的异常";
            }
            else if (ex is ReflectionTypeLoadException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.ReflectionTypeLoad;
                res = "当模块中的任何类无法加载时由 Module.GetTypes 方法引发的异常";
            }
            else if (ex is SqlException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Sql;
                res = "当SQLServer返回警告或错误时引发的异常。";
            }
            else if (ex is ServerException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Server;
                res = "当客户端连接无法引发异常的非 .NET 框架应用程序时，为向客户端传达错误而引发的异常";
            }
            else if (ex is SecurityException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Security;
                res = "检测到安全性错误时引发的异常";
            }
            else if (ex is SynchronizationLockException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.SynchronizationLock;
                res = "在从非同步的代码块中调用同步方法时引发的异常";
            }
            else if (ex is SoapException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Soap;
                res = "当通过SOAP调用 XML Web services 方法且出现异常时引发的异常";
            }
            else if (ex is ThreadAbortException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.ThreadAbort;
                res = "在对Abort方法进行调用时引发的异常";
            }
            else if (ex is TypeLoadException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.TypeLoad;
                res = "类型加载失败发生时引发的异常";
            }
            else if (ex is TypeUnloadedException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.TypeUnloaded;
                res = "试图访问已卸载的类时引发的异常";
            }
            else if (ex is XmlSchemaException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.XmlSchema;
                res = "返回关于架构异常的详细信息";
            }
            else if (ex is XmlException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Xml;
                res = "返回有关最后一个异常的详细信息";
            }
            else if (ex is XsltException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.Xslt;
                res = "由于在处理“可扩展样式表语言”(XSL) 转换时发生错误而引发的异常";
            }
            else if (ex is XPathException)
            {
                extype = (HttpStatusCode)HttpStatusCodeMore.XPath;
                res = "处理 XPath 表达式而发生错误时引发的异常";
            }
            //.....这里可以根据项目需要返回到客户端特定的状态码。如果找不到相应的异常，统一返回服务端错误500
            else
            {
                 
                res = "服务器上发生了一般错误";
                extype = (HttpStatusCode.InternalServerError);
            }
            sList.Add( extype,res);
            return sList;
        }
    }
    public enum HttpStatusCodeMore
    {

        // 摘要: 
        //     等效于 HTTP 状态 1。  指示客户端可能继续其请求。 System.ArrayTypeMismatchException 当存储一个数组时，如果由于被存储的元素的实际类型与数组的实际类型不兼容而导致存储失败，就会引发此异常。
        ArrayTypeMismatch = 61,
        //
        // 摘要: 
        //     等效于 HTTP 状态2。 System.DivideByZeroException  在试图用零除整数值时引发。。
        DivideByZero = 2,
        //
        // 摘要: 
        //     等效于 HTTP 状态 3。 System.IndexOutOfRangeException在试图使用小于零或超出数组界限的下标索引数组时引发。
        IndexOutOfRange = 3,
        //
        // 摘要: 
        //     等效于 HTTP 状态 4。 System.InvalidCastException当从基类型或接口到派生类型的显式转换在运行时失败时，就会引发此异常。
        InvalidCast = 4,
        //
        // 摘要: 
        //     等效于 HTTP 状态 5。System.NullReferenceException在需要使用引用对象的场合，如果使用 null 引用，就会引发此异常。
        NullReference = 5,
        //
        // 摘要: 
        //     等效于 HTTP 状态 6。 System.OutOfMemoryException在分配内存（通过 new）的尝试失败时引发。
        OutOfMemory = 6,
        //
        // 摘要: 
        //     等效于 HTTP 状态 7。 System.OverflowException在 checked 上下文中的算术运算溢出时引发。
        Overflow = 7,
        //
        // 摘要: 
        //     等效于 HTTP 状态 8。 System.StackOverflowException当执行堆栈由于保存了太多挂起的方法调用而耗尽时，就会引发此异常；这通常表明存在非常深或无限的递归。
        StackOverflow = 8,
        //
        // 摘要: 
        //     等效于 HTTP 状态 9。 System.TypeInitializationException在静态构造函数引发异常并且没有可以捕捉到它的 catch 子句时引发。
        TypeInitialization = 9,
        //
        // 摘要: 
        //     等效于 HTTP 状态 9。 System.AccessViolationExceptio尝试读取或写入受保护的内存。
        AccessViolation = 10,
        //
        // 摘要: 
        //     等效于 HTTP 状态 1。 System.AggregateException聚合异常
        Aggregate = 11,
        //
        // 摘要: 
        //     等效于 HTTP 状态 12。 System.AppDomainUnloadedException在尝试访问已卸载的应用程序域时引发的异常
        AppDomainUnloaded = 12,
        //
        // 摘要: 
        //     等效于 HTTP 状态 13。 System.ApplicationException Application Exception
        Application = 13,
        //
        // 摘要: 
        //     等效于 HTTP 状态 14。 System.ArgumentException 在向方法提供的其中一个参数无效时引发的异常
        Argument = 14,
        //
        // 摘要: 
        //     等效于 HTTP 状态 15。 System.ArgumentNullException 值不能为 null
        ArgumentNull = 15,
        //
        // 摘要: 
        //     等效于 HTTP 状态 16。 System.ArithmeticException 因算术运算、类型转换或转换操作中的错误而引发的异常
        Arithmetic = 16,
        //
        // 摘要: 
        //     等效于 HTTP 状态 17。 System.ArgumentOutOfRangeException 指定的参数已超出有效值的范围
        ArgumentOutOfRange = 17,
        //
        // 摘要: 
        //     等效于 HTTP 状态 18。 System.BadImageFormatException  当 DLL 或可执行程序的文件图像无效时引发的异常
        BadImageFormat = 18,
        //
        // 摘要: 
        //     等效于 HTTP 状态 19。 System.CannotUnloadAppDomainException  卸载应用程序域的尝试失败时引发的异常
        CannotUnloadAppDomain = 19,
        //
        // 摘要: 
        //     等效于 HTTP 状态 20。 System.ContextMarshalException  在尝试将对象封送过上下文边界失败时引发的异常
        ContextMarshal = 20,
        //
        // 摘要: 
        //     等效于 HTTP 状态 21。 System.CookielException  cookie异常 
        Cookie = 21,
        //
        // 摘要: 
        //     等效于 HTTP 状态 22。 System.DataMisalignedException  DataMisalignedException异常 
        DataMisaligned = 22,
        //
        // 摘要: 
        //     等效于 HTTP 状态 23。 System.DllNotFoundException  无法加载DLL异常 
        DllNotFound = 23,
        //
        // 摘要: 
        //     等效于 HTTP 状态 24。 System.DuplicateWaitObjectException  重复等待对象异常
        DuplicateWaitObject = 24,
        //
        // 摘要: 
        //     等效于 HTTP 状态 25。 System.DuplicateWaitObjectException  配置设置中发生错误时引发的异常
        Configuration = 25,
        //
        // 摘要: 
        //     等效于 HTTP 状态 26。 System.DataException   表示使用 ADO.NET 组件发生错误时引发的异常
        Data = 26,
        //
        // 摘要: 
        //     等效于 HTTP 状态 27。 System.DBConcurrencyException 在更新操作过程中受影响的行数等于零时，由 DataAdapter 所引发的异常
        DBConcurrency = 27,
        //
        // 摘要: 
        //     等效于 HTTP 状态 28。 System.DuplicateNameException 重复的名称例外
        DuplicateName = 28,
        //
        // 摘要: 
        //     等效于 HTTP 状态 29。 System.EntryPointNotFoundException 未发现入口点
        EntryPointNotFound = 29,
        //
        // 摘要: 
        //     等效于 HTTP 状态 30。 System.ExecutionEngineException ExecutionEngineException
        ExecutionEngine = 30,
        //
        // 摘要: 
        //     等效于 HTTP 状态 31。 System.FieldAccessException FieldAccessException
        FieldAccess = 31,
        //
        // 摘要: 
        //     等效于 HTTP 状态 32。 System.FormatException  当参数格式不符合调用的方法的参数规范时引发的异常
        Format = 32,
        //
        // 摘要: 
        //     等效于 HTTP 状态 33。 System.HttpCompileException  HTTP编译异常
        HttpCompile = 33,
        //
        // 摘要: 
        //     等效于 HTTP 状态 34。 System.HttpCompileException  HTTPHttp
        Http = 34,
        //
        // 摘要: 
        //     等效于 HTTP 状态 35。 System.HttpRequestValidationException  从客户端中检测到有潜在危险的 Request.Form 值
        HttpRequestValidation = 35,
        //
        // 摘要: 
        //     等效于 HTTP 状态 36。 System.HttpUnhandledException  HttpUnhandledException
        HttpUnhandled = 36,
        //
        // 摘要: 
        //     等效于 HTTP 状态 376。 System.InstallException  在安装的提交、回滚或卸载阶段发生错误时引发的异常
        Install = 37,
        //
        // 摘要: 
        //     等效于 HTTP 状态 38。 System.InvalidPrinterException  表示当试图用无效的打印机设置来访问打印机时所引发的异常
        InvalidPrinter = 38,
        //
        // 摘要: 
        //     等效于 HTTP 状态 39。 System.InvalidOperationException 当方法调用对于对象的当前状态无效时引发的异常
        InvalidOperation = 39,
        //
        // 摘要: 
        //     等效于 HTTP 状态 40。 System.InternalBufferOverflowException 内部缓冲区溢出时引发的异常
        InternalBufferOverflow = 40,
        //
        // 摘要: 
        //     等效于 HTTP 状态 41。 System.IOException 发生 I/O 错误时引发的异常
        IO = 41,
        //
        // 摘要: 
        //     等效于 HTTP 状态 42。 System.LicenseException 表示当组件不能被授予许可证时引发的异常
        License = 42,
        //
        // 摘要: 
        //     等效于 HTTP 状态 43。 System.ManagementException 表示管理异常
        Management = 43,
        //
        // 摘要: 
        //     等效于 HTTP 状态 44。 System.NotImplementedException 在无法实现请求的方法或操作时引发的异常
        NotImplemented = 44,
        //
        // 摘要: 
        //     等效于 HTTP 状态 44。 System.NotSupportedException 当调用的方法不受支持，或试图读取、查找或写入不支持调用功能的流时引发的异
        NotSupported = 45,
        //
        // 摘要: 
        //     等效于 HTTP 状态 46。 System. PolicyException 当策略禁止代码运行时引发的异常
        Policy = 46,
        //
        // 摘要: 
        //     等效于 HTTP 状态 47。 System.RankException 将维数错误的数组传递给方法时引发的异常
        Rank = 47,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.ReflectionTypeLoadException 当模块中的任何类无法加载时由 Module.GetTypes 方法引发的异常
        ReflectionTypeLoad = 48,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.SqlException 当 SQL Server 返回警告或错误时引发的异常。
        Sql = 49,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.ServerException 当客户端连接无法引发异常的非 .NET 框架应用程序时，为向客户端传达错误而引发的异常
        Server = 50,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.SecurityException 检测到安全性错误时引发的异常
        Security = 51,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.SynchronizationLockException 在从非同步的代码块中调用同步方法时引发的异常
        SynchronizationLock = 52,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.SoapException 当通过 SOAP 调用 XML Web services 方法且出现异常时引发的异常
        Soap = 53,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System. ThreadAbortException 在对 Abort 方法进行调用时引发的异常
        ThreadAbort = 54,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.TypeLoadException 类型加载失败发生时引发的异常
        TypeLoad = 55,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.TypeUnloadedException 试图访问已卸载的类时引发的异常
        TypeUnloaded = 56,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.XmlSchemaException 返回关于架构异常的详细信息
        XmlSchema = 57,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.XmlException 返回有关最后一个异常的详细信息
        Xml = 58,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.XsltException 由于在处理“可扩展样式表语言”(XSL) 转换时发生错误而引发的异常
        Xslt = 59,
        //
        // 摘要: 
        //     等效于 HTTP 状态 48。 System.XPathException 处理 XPath 表达式而发生错误时引发的异常
        XPath = 60






        
    }
}