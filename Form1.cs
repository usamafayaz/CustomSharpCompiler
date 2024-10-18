using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Form1 : Form
    {
        List<SymbolTable> symbolList = new List<SymbolTable>();
        public Form1()
        {
            InitializeComponent();
            this.ControlBox = true;
        }
        private void compilebtn_Click(object sender, EventArgs e)
        {
            txtMessage.Clear();
            symbolList.Clear();
            for (int i = 0; i < txtInput.Lines.Count(); i++)
            {
                try
                {
                    // declare integar|decimal|match|key X;   declare x =5; declare x =5;
                    if (Regex.IsMatch(txtInput.Lines[i], @"^declare\s+(integar|decimal|match|key|string)*\s+(([A-Za-z][A-Za-z0-9_]*)|([][A-Za-z][A-Za-z0-9]*))(|[\s]{0,},[\s]{0,}[A-Za-z_][A-Za-z0-9]*)*[;]$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(' ');
                        List<string> temp = new List<string>();
                        temp.Add(identifierArray[2]);
                        recursive(temp, identifierArray[1]);
                        for (int k = 0; k < symbolList.Count(); k++)
                        {
                            for (int m = k+1; m < symbolList.Count(); m++)
                            {
                                if (symbolList[k].datatype == symbolList[m].datatype && symbolList[k].identifier == symbolList[m].identifier)
                                {
                                    txtMessage.Text += "Redefenition of variable " + symbolList[m].identifier + Environment.NewLine;
                                }
                                else if (symbolList[k].identifier == symbolList[m].identifier)
                                {
                                    txtMessage.Text += "Redefenition of variable " + symbolList[m].identifier + Environment.NewLine;
                                }
                            }
                        }
                    }
                    //declare integar x=5;
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^declare\s+(integar)*\s+([A-Za-z][A-Za-z0-9_]*)\s*=\s*[0-9]+[;]$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { ' ', ' ','=' });
                        string identifier = identifierArray[2].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = identifier,
                            datatype = "integar"
                        });
                    }
                    //declare decimal x=5.5;
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^declare\s+(decimal)*\s+([A-Za-z][A-Za-z0-9_]*)\s*=\s*[0-9]+\.[0-9]+[;]$")) 
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { ' ', ' ', '=' });
                        string identifier = identifierArray[2].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = identifier,
                            datatype = "decimal"
                        });
                    }
                    //declare match x=true;
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^declare\s+(match)*\s+([A-Za-z][A-Za-z0-9_]*)\s*=\s*(true|false)[;]$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { ' ', ' ', '=' });
                        string identifier = identifierArray[2].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = identifier,
                            datatype = "match"
                        });
                    }
                    //declare key x='5';
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^declare\s+(key)*\s+([A-Za-z][A-Za-z0-9_]*)\s*=\s*\'([A-Za-z0-9_]){1,1}\'[;]$")) 
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { ' ', ' ', '=' });
                        string identifier = identifierArray[2].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = identifier,
                            datatype = "key"
                        });
                    }
                    //declare string x="Hello World";
                    else if (Regex.IsMatch(txtInput.Lines[i], "^declare\\s+(string)*\\s+([A-Za-z][A-Za-z0-9_]*)\\s*=\\s*\".*\"+[;]$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { ' ', ' ', '=' });
                        string identifier = identifierArray[2].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = identifier,
                            datatype = "string"
                        });
                    }
                    else if (Regex.IsMatch(txtInput.Lines[i], "^[A-Za-z_][A-Za-z0-9_]*\\s+(inc|dec);$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(' ');
                        string identifier = identifierArray[0].Trim();
                        if (!isdefinition(identifier))
                        {
                            txtMessage.Text += "Undeclared variable " + identifier + Environment.NewLine;
                        }
                    }

                    // X=5; Gives Error if X is not Declared
                    else if (Regex.IsMatch(txtInput.Lines[i], "^[A-Za-z_][A-Za-z0-9_]*[\\s]*=[\\s]*(([0-9]+)|([0-9]+.[0-9]+)|(\"[A-Za-z0-9\\s]+\")|(\'[A-Za-z]{1}\')|(true|false));$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split('=');
                        string identifier = identifierArray[0].Trim();
                        string value = identifierArray[1].Trim();
                        string actualValue = getValueFromCommaSeprated(value);

                        if (isdefinition(identifier))
                        {
                            foreach (var j in symbolList)
                            {
                                if (j.identifier == identifier)
                                {
                                    if (j.datatype == "integar")
                                    {
                                        if (Regex.IsMatch(actualValue, @"[0-9]+"))
                                        {

                                        }
                                        else
                                        {
                                            throw new Exception("Data Type Mismatched");
                                        }
                                    }
                                    if (j.datatype == "string")
                                    {
                                        if (Regex.IsMatch(actualValue, "^\".*\"$"))
                                        {

                                        }
                                        else
                                        {
                                            throw new Exception("Data Type Mismatched");
                                        }
                                    }
                                    if (j.datatype == "match")
                                    {
                                        if (Regex.IsMatch(actualValue, "^(true|false)$"))
                                        {

                                        }
                                        else
                                        {
                                            throw new Exception("Data Type Mismatched");
                                        }
                                    }
                                    if (j.datatype == "decimal")
                                    {
                                        if (Regex.IsMatch(actualValue, "^[0-9]+\\.[0-9]+$"))
                                        {

                                        }
                                        else
                                        {
                                            throw new Exception("Data Type Mismatched");
                                        }
                                    }
                                    if (j.datatype == "key")
                                    {
                                        if (Regex.IsMatch(actualValue, "^\\'([A-Za-z0-9_]){1,1}\\'$"))
                                        {

                                        }
                                        else
                                        {
                                            throw new Exception("Data Type Mismatched");
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            txtMessage.Text += "Undeclared variable " + identifier + Environment.NewLine;
                        }
                    }
                    else if (Regex.IsMatch(txtInput.Lines[i], "^[a-zA-Z_][a-zA-Z_0-9]*='(\\\\n|\\\\r|\\\\t)';$"))
                    {

                    }
                    // out("Hello World");
                    // out(X); Gives Error if X is not Declared

                    else if (Regex.IsMatch(txtInput.Lines[i], "^out\\((((\"[A-Za-z0-9\\s]{1,}\")|([A-Za-z_][A-Za-z0-9_]*|[0-9]+))|'(\\\\n|\\\\r|\\\\t)')\\);$"))
                    {
                        string[] extractedVar = txtInput.Lines[i].Split('(');
                        foreach (var item in extractedVar)
                        {
                            var variable = item;
                            if (item.EndsWith(";"))
                            {
                                string[] temp = item.Split(')');
                                variable = temp[0];
                            }

                            if (Regex.IsMatch(variable, "^[A-Za-z_][A-Za-z0-9_]*$") && item != "out")
                            {
                                bool hello = isdefinition(variable);
                                if (!hello)
                                {
                                    txtMessage.Text += "Undeclared variable " + variable + Environment.NewLine;
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                    // out(a+5+b);  Gives Error if a and b are not Declared.
                    else if (Regex.IsMatch(txtInput.Lines[i], "^out\\(([A-Za-z_][A-Za-z0-9_]*|[0-9]+)((\\+|\\*|-|/)([A-Za-z_][A-Za-z0-9_]*|[0-9]+))+\\);$"))
                    {
                        string[] extractedVar1 = txtInput.Lines[i].Split('(');
                        string[] extractedVar2 = extractedVar1[1].Split(')');
                        string[] variables = extractedVar2[0].Split('+');
                        foreach (var item in variables)
                        {
                            var vari = item;
                            if (Regex.IsMatch(vari, "^[A-Za-z_][A-Za-z0-9_]*$"))
                            {
                                bool flag = isdefinition(vari);
                                if (!flag)
                                    txtMessage.Text += "Undeclared variable " + vari + Environment.NewLine;
                            }
                            else { }
                        }
                    }
                    // in X // Input in X  //Gives Error if X is not Declared

                    else if (Regex.IsMatch(txtInput.Lines[i], "^in\\s+[A-Za-z][A-Za-z0-9_]*[;]$"))
                    {
                        string[] extractedVar = txtInput.Lines[i].Split(' ');
                        string variable = getValueFromCommaSeprated(extractedVar[1].Trim());
                        bool flag = isdefinition(variable);
                        if (!flag)
                        {
                            txtMessage.Text += "Undeclared variable " + variable + Environment.NewLine;
                        }
                    }
                    // x=5+5/5; 
                    else if (Regex.IsMatch(txtInput.Lines[i], "^[A-Za-z_][A-Za-z0-9_]*=[0-9]+((\\+|\\*|-|/)[0-9]+)+;$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split('=');
                        string identifier = identifierArray[0].Trim();
                        if (isdefinition(identifier))
                        {

                        }
                        else
                        {
                            txtMessage.Text += "Undeclared variable " + identifier + Environment.NewLine;
                        }
                    }
                    // x = 5<=5;
                    else if (Regex.IsMatch(txtInput.Lines[i], "^[A-Za-z_][A-Za-z0-9_]*=[0-9]+((<|>|!|<=|>=|!=|==)[0-9]+)+;$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split('=');
                        string identifier = identifierArray[0].Trim();
                        if (isdefinition(identifier))
                        {

                        }
                        else
                        {
                            txtMessage.Text += "Undeclared variable " + identifier + Environment.NewLine;
                        }
                    }
                    else if (Regex.IsMatch(txtInput.Lines[i], "^agar\\((([A-Za-z_][A-Za-z0-9_]*)|([0-9]+))\\s+(<|>|!|<=|>=|!=|==)(([A-Za-z_][A-Za-z0-9_]*)|([0-9]+)){}magar{}$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { '(', ' ' });
                        string identifier = identifierArray[1].Trim();
                        if (isdefinition(identifier))
                        {

                        }
                        else
                        {
                            txtMessage.Text += "Undeclared variable " + identifier + Environment.NewLine;
                        }
                    }
                    // agar(a <4){}magar{}   gives error if a is not declared
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^agar\((([A-Za-z_][A-Za-z0-9_]*)|([0-9]+))\s+(<|>|!|<=|>=|!=|==)(([A-Za-z_][A-Za-z0-9_]*)|([0-9]+))\)\{\}\s*(magar agar\((([A-Za-z_][A-Za-z0-9_]*)|([0-9]+))(<|>|!|<=|>=|!=|==)(([A-Za-z_][A-Za-z0-9_]*)|([0-9]+))\){})?\s*(magar\s*{})?$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { '(', ' ' });
                        string identifier = identifierArray[1].Trim();
                        if (isdefinition(identifier))
                        {

                        }
                        else
                        {
                            txtMessage.Text += "Undeclared variable " + identifier + Environment.NewLine;
                        }
                    }
                    // repeat(i=5;i<5;i inc)  gives error if i is not declared
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^repeat\(([A-Za-z_][A-Za-z0-9_]*=(([0-9]+)|([A-Za-z_][A-Za-z0-9_]*)))?;[A-Za-z_][A-Za-z0-9_]*(<|>|!|<=|>=|!=|==)(([0-9]+)|([A-Za-z_][A-Za-z0-9_]*));([A-Za-z_][A-Za-z0-9_]*\s+(inc|dec))?\){}$"))
                    {
                        string[] identifierArray = txtInput.Lines[i].Split(new char[] { '(', '=' });
                        string identifier = identifierArray[1].Trim();
                        if (isdefinition(identifier))
                        {

                        }
                        else
                        {
                            txtMessage.Text += "Undeclared variable " + identifier + Environment.NewLine;
                        }
                    }
                    // repeat(declare integar i=5;i<5;i inc)  gives error if i is not declared
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^repeat\(declare\s+integar\s([A-Za-z_][A-Za-z0-9_]*=(([0-9]+)|([A-Za-z_][A-Za-z0-9_]*)))?;[A-Za-z_][A-Za-z0-9_]*(<|>|!|<=|>=|!=|==)(([0-9]+)|([A-Za-z_][A-Za-z0-9_]*));([A-Za-z_][A-Za-z0-9_]*\s+(inc|dec))?\){}$"))
                    {

                    }
                    // exec f1();  Gives error if f1 is not declared.
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^exec\s+([A-Za-z_][A-Za-z0-9_]*)\(\);$"))
                    {
                        string[] functions = txtInput.Lines[i].Split(new char[] { ' ', '(' });

                        string functionName = functions[1].Trim();
                        if (isdefinition(functionName))
                        {

                        }
                        else
                        {
                            txtMessage.Text += "Undeclared Fucntion " + functionName + Environment.NewLine;
                        }
                    }

                    // integar array[10];
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^(integar|decimal|match|key)\s*[A-Za-z_][A-Za-z0-9_]*\[[1-9][0-9]*\];"))
                    {
                        string[] array = txtInput.Lines[i].Split(new char[] { ' ', '[' });
                        string arrayName = array[1].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = arrayName,
                            datatype = "array"
                        });
                    }

                    // integar array={1,2,3,4};
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^(integar|decimal|match|key)\s*[A-Za-z_][A-Za-z0-9_]*\[\]=\{[0-9]+((,([0-9])+)*)?\};"))
                    {
                        string[] array = txtInput.Lines[i].Split(new char[] { ' ', '=' });
                        string arrayName = array[1].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = arrayName,
                            datatype = "array"
                        });
                    }
                    // array[4]=5;  // throws error if array is not declared
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^[A-Za-z_][A-Za-z0-9_]*\[[0-9]+\]=[0-9]+;"))
                    {
                        string arrayName = txtInput.Lines[i].Split('[')[0];
                        if (isdefinition(arrayName))
                        {
                        }
                        else
                        {
                            txtMessage.Text += "Undeclared Array " + arrayName + Environment.NewLine;
                        }

                    }

                    //declare Sum(integar x, decimal y){return 5;}
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^declare\s+[A-Za-z_][A-Za-z0-9_]*\((((integar\s|decimal\s|match\s|key\s)?[A-Za-z_][A-Za-z0-9_]*)(,\s*(integar\s|decimal\s|match\s|key\s)?[A-Za-z_][A-Za-z0-9_]*)*)?\){((return;)|(return\s[1-9]+;)|(return\strue;)|(return\sfalse;))?}"))
                    {
                        FunctionNameError(txtInput.Lines[i], i + 1);
                        string[] functions = txtInput.Lines[i].Split(new char[] { ' ', '(' });
                        string functionName = functions[1].Trim();
                        symbolList.Add(new SymbolTable
                        {
                            identifier = functionName,
                            datatype = "function"
                        });
                    }
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^(integar|decimal|key|string|match)\s*\*\s+[A-Za-z_][A-Za-z0-9_]*\s*=\s*new\s+(integar|decimal|key|string|match)\s*\[[1-9][0-9]*\];$"))
                    {
                        string[] split = txtInput.Lines[i].Split(new char[] { ' ', '[' });
                        if (split[0] != split[5])
                        {
                            txtMessage.Text = "Data type Mismatched !";
                        }
                    }
                    else if (Regex.IsMatch(txtInput.Lines[i], @"^create\s*[A-Za-z_][A-Za-z0-9_]*{}$"))
                    {

                    }
                    else
                    {
                        txtMessage.Text += "An Error Occured in Line: " + (i + 1) + Environment.NewLine;
                    }
                }
                catch (Exception ex)
                {
                    txtMessage.Text += ex.Message;
                }
            }
        }
        private void recursive(List<string> arr, string data)
        {
            string[] list;
            foreach (var item in arr)
            {
                list = item.Split(',');
                foreach (var i in list)
                {
                    if (i.EndsWith(";"))
                    {
                        var ip = i.Split(';');
                        symbolList.Add(new SymbolTable { identifier = ip[0].ToString(), datatype = data });
                        return;
                    }
                    else
                    {
                        symbolList.Add(new SymbolTable { identifier = i.ToString(), datatype = data });
                    }
                }
            }

        }
        private bool redefinition(string variable)
        {
            foreach (var i in symbolList)
            {
                if (i.identifier != variable)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        private bool isdefinition(string variable)
        {
            foreach (var i in symbolList)
            {
                if (i.identifier == variable)
                {
                    return true;
                }

            }
            return false;
        }
        private string getValueFromCommaSeprated(string variable)
        {
            string[] value = variable.Split(';');
            return value[0];
        }
        private void FunctionNameError(string variable, int lineIndex)
        {
            string fun_name = variable.Split('(')[0];
            string actualName = fun_name.Split(' ')[1];
            if (actualName == "integar" || actualName == "decimal" || actualName == "match" || actualName == "key")
            {
                throw new Exception("Function Name cannot be Reserved Word.");
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
