﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.CSharp
Imports Microsoft.CodeAnalysis.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Symbols.Metadata

    Public Class InAttributeModifierTests
        Inherits BasicTestBase

        <Fact>
        Public Sub ReadOnlySignaturesAreRead_Methods_Parameters_Static()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    public static void M(ref readonly int x)
    {
        System.Console.WriteLine(x);
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim x = 5
        TestRef.M(x)
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="5")
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreRead_Methods_Parameters_NoModifiers()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    public void M(ref readonly int x)
    {
        System.Console.WriteLine(x);
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim x = 5
        Dim obj = New TestRef()
        obj.M(x)
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="5")
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreRead_Indexers_Parameters_NoModifiers()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    public int this[ref readonly int p]
    {
        set
        {
            System.Console.WriteLine(p);
        }
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim x = 5
        Dim obj = New TestRef()
        obj(x) = 0
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="5")
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreRead_Constructors()
            Dim reference = CreateCSharpCompilation("
public struct TestRef
{
    public TestRef(ref readonly int value)
    {
        System.Console.WriteLine(value);
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj1 = New TestRef(4)
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="4")
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Methods_Parameters_Virtual()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    public virtual void M(ref readonly int x)
    {
        System.Console.WriteLine(x);
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim x = 5
        Dim obj = New TestRef()
        obj.M(x)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'M' has a return type that is not supported or parameter types that are not supported.
        obj.M(x)
            ~
</expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Methods_Parameters_Abstract()
            Dim reference = CreateCSharpCompilation("
public abstract class TestRef
{
    public abstract void M(ref readonly int x);
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(obj As TestRef)
        Dim x = 5
        obj.M(x)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'M' has a return type that is not supported or parameter types that are not supported.
        obj.M(x)
            ~
</expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Methods_ReturnTypes_Virtual()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 0;
    public virtual ref readonly int M()
    {
        return ref value;
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        obj.M()
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'M' has a return type that is not supported or parameter types that are not supported.
        obj.M()
            ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Methods_ReturnTypes_Abstract()
            Dim reference = CreateCSharpCompilation("
public abstract class TestRef
{
    public abstract ref readonly int M();
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(obj As TestRef) 
        obj.M()
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'M' has a return type that is not supported or parameter types that are not supported.
        obj.M()
            ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Methods_ReturnTypes_Static()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private static int value = 0;
    public static ref readonly int M()
    {
        return ref value;
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        TestRef.M()
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'M' has a return type that is not supported or parameter types that are not supported.
        TestRef.M()
                ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Methods_ReturnTypes_NoModifiers()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 0;
    public ref readonly int M()
    {
        return ref value;
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        obj.M()
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'M' has a return type that is not supported or parameter types that are not supported.
        obj.M()
            ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Properties_Virtual()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 0;
    public virtual ref readonly int P => ref value;
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        Dim value = obj.P
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'P' is of an unsupported type.
        Dim value = obj.P
                        ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Properties_Static()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private static int value = 0;
    public static ref readonly int P => ref value;
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim value = TestRef.P
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'P' is of an unsupported type.
        Dim value = TestRef.P
                            ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Properties_NoModifiers()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 0;
    public ref readonly int P => ref value;
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        Dim value = obj.P
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'P' is of an unsupported type.
        Dim value = obj.P
                        ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Properties_Abstract()
            Dim reference = CreateCSharpCompilation("
public abstract class TestRef
{
    public abstract ref readonly int P { get; }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(obj As TestRef) 
        Dim value = obj.P
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'P' is of an unsupported type.
        Dim value = obj.P
                        ~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Indexers_Parameters_Virtual()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    public virtual int this[ref readonly int p] => 0;
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim p = 0
        Dim obj = New TestRef()
        Dim value = obj(p)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'TestRef.Item(p As )' is of an unsupported type.
        Dim value = obj(p)
                    ~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Indexers_Parameters_Abstract()
            Dim reference = CreateCSharpCompilation("
public abstract class TestRef
{
    public abstract int this[ref readonly int p] { set; }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(obj As TestRef) 
        Dim p = 0
        Dim value = obj(p)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'TestRef.Item(p As )' is of an unsupported type.
        Dim value = obj(p)
                    ~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Indexers_ReturnTypes_Virtual()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 0;
    public virtual ref readonly int this[int p] => ref value;
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        Dim value = obj(0)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'TestRef.Item(p As )' is of an unsupported type.
        Dim value = obj(0)
                    ~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Indexers_ReturnTypes_Abstract()
            Dim reference = CreateCSharpCompilation("
public abstract class TestRef
{
    public abstract ref readonly int this[int p] { get; }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(obj As TestRef) 
        Dim value = obj(0)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'TestRef.Item(p As )' is of an unsupported type.
        Dim value = obj(0)
                    ~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub ReadOnlySignaturesAreNotSupported_Indexers_ReturnTypes_NoModifiers()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 0;
    public ref readonly int this[int p] => ref value;
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        Dim value = obj(0)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30643: Property 'TestRef.Item(p As )' is of an unsupported type.
        Dim value = obj(0)
                    ~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub UsingLambdasOfRefReadOnlyDelegatesIsNotSupported_Invoke_Parameters()
            Dim reference = CreateCSharpCompilation("
public delegate void D(ref readonly int p);
", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(lambda As D) 
        Dim x = 0
        lambda(x)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'D' has a return type that is not supported or parameter types that are not supported.
        lambda(x)
        ~~~~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub UsingLambdasOfRefReadOnlyDelegatesIsNotSupported_BeginInvoke_Parameters()
            Dim reference = CreateCSharpCompilation("
public delegate void D(ref readonly int p);
", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(lambda As D) 
        Dim x = 0
        lambda.BeginInvoke(x, Nothing, Nothing)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'BeginInvoke' has a return type that is not supported or parameter types that are not supported.
        lambda.BeginInvoke(x, Nothing, Nothing)
               ~~~~~~~~~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub UsingLambdasOfRefReadOnlyDelegatesIsNotSupported_EndInvoke_Parameters()
            Dim reference = CreateCSharpCompilation("
public delegate void D(ref readonly int p);
", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(lambda As D) 
        Dim x = 0
        lambda.EndInvoke(x)
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'EndInvoke' has a return type that is not supported or parameter types that are not supported.
        lambda.EndInvoke(x)
               ~~~~~~~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub UsingLambdasOfRefReadOnlyDelegatesIsNotSupported_Invoke_ReturnTypes()
            Dim reference = CreateCSharpCompilation("
public delegate ref readonly int D();
", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(lambda As D) 
        Dim x = lambda()
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'D' has a return type that is not supported or parameter types that are not supported.
        Dim x = lambda()
                ~~~~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub UsingLambdasOfRefReadOnlyDelegatesIsNotSupported_EndInvoke_ReturnTypes()
            Dim reference = CreateCSharpCompilation("
public delegate ref readonly int D();
", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main(lambda As D) 
        lambda.EndInvoke()
    End Sub
End Class
    </file>
                </compilation>

            Dim compilation = CreateCompilationWithMscorlib(source, references:={reference})

            AssertTheseDiagnostics(compilation, <expected>
BC30657: 'EndInvoke' has a return type that is not supported or parameter types that are not supported.
        lambda.EndInvoke()
               ~~~~~~~~~
                                                </expected>)
        End Sub

        <Fact>
        Public Sub OverloadResolutionShouldBeAbleToPickOverloadsWithNoModreqsOverOnesWithModreq_Methods_Parameters()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    public virtual void PrintMul(ref readonly int x)
    {
        System.Console.WriteLine(x * 2);
    }
    public void PrintMul(ref readonly long x)
    {
        System.Console.WriteLine(x * 4);
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim value As Integer = 5
        Dim obj = New TestRef()
        obj.PrintMul(value)
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="20")
        End Sub

        <Fact>
        Public Sub OverloadResolutionShouldBeAbleToPickOverloadsWithNoModreqsOverOnesWithModreq_Methods_ReturnTypes()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 5;

    public ref readonly int PrintMul(int x)
    {
        value = value * 2;
        return ref value;
    }
    public int PrintMul(long x)
    {
        value = value * 4;
        return value;
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        System.Console.WriteLine(obj.PrintMul(0))
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="20")
        End Sub

        <Fact>
        Public Sub OverloadResolutionShouldBeAbleToPickOverloadsWithNoModreqsOverOnesWithModreq_Indexers_Parameters()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    public virtual int this[ref readonly int x]
    {
        set
        {
            System.Console.WriteLine(x * 2);
        }
    }
    public int this[ref readonly long x]
    {
        set
        {
            System.Console.WriteLine(x * 4);
        }
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim value As Integer = 5
        Dim obj = New TestRef()
        obj(value) = 0
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="20")
        End Sub

        <Fact>
        Public Sub OverloadResolutionShouldBeAbleToPickOverloadsWithNoModreqsOverOnesWithModreq_Indexers_ReturnTypes()
            Dim reference = CreateCSharpCompilation("
public class TestRef
{
    private int value = 5;
    public ref readonly int this[int x]
    {
        get
        {
            value = value * 2;
            return ref value;
        }
    }
    public int this[long x]
    {
        get
        {
            value = value * 4;
            return value;
        }
    }
}", parseOptions:=New CSharpParseOptions(CSharp.LanguageVersion.Latest)).EmitToImageReference()

            Dim source =
                <compilation>
                    <file>
Class Test
    Shared Sub Main() 
        Dim obj = New TestRef()
        System.Console.WriteLine(obj(0))
    End Sub
End Class
    </file>
                </compilation>

            CompileAndVerify(source, additionalRefs:={reference}, expectedOutput:="20")
        End Sub
    End Class

End Namespace