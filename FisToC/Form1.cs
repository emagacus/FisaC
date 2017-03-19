using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FisToC
{

    
   

    public class Input
        {

        }

    public class Output
    {

    }

    public class Rules
    {

        public List<string> rulestring;
        public List<int> inputIndex;
        public List<int> OutputIndex;
        public Rules() { rulestring = new List<string>(); }

        public int[,] ruleArray(int nrules,int ninput, int noutput)
        {
            int columns = ninput + noutput;
            int[,] reglas = new int[nrules,columns];

            int filecount = 0;
            foreach (string s in rulestring)
            {
                
                string[] array = s.Split(' ');
                for(int x=0;x<columns;x++)
                {
                    array[x] = array[x].Trim(',');
                    Console.WriteLine(array[x]);
                    reglas[filecount, x] = int.Parse(array[x]);
                    
                }
                filecount++;
                if (filecount == nrules) { break; }
            }

            Console.WriteLine(reglas.ToString());
            return reglas;
        } 

    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void processingText()
        {
            string[] lineas = textBox1.Text.Split('|');
            int InputNumber=0;
            int Outputnumber = 0;
            int NumRules = 0;
            int index = 0;
            Rules Reglas = new Rules();
            List<Input> inputs = new List<Input>();
            List<Output> outputs = new List<Output>();

            foreach (string s in lineas)
            {
                if(s.Contains("Input"+(InputNumber+1).ToString()))
                {
                    Input inp = new Input();
                    inputs.Add(inp);
                    InputNumber++;
                }
                if(s.Contains("Output"+(Outputnumber+1).ToString()))
                {
                    Output ou = new Output();
                    outputs.Add(ou);
                    Outputnumber++;
                }
                if (s.Contains("NumRules"))
                {
                    string[] nr = lineas[index].Split('=');
                    NumRules = int.Parse(nr[1]);
                }

                    if (s.Contains("[Rules]"))
                {
                   
                    for(int y=index+1;y<lineas.Length;y++)
                    {
                        lineas[y]=lineas[y].Trim(',');
                        Reglas.rulestring.Add(lineas[y]);
                        Console.WriteLine(lineas[y]);
                    }

                }
                index++;
            }
            /*
            textBox2.Text = "Numero De inputs: " + InputNumber.ToString() + '\n';
            textBox2.Text += "Numero De inputs arr: " + inputs.Count.ToString() + '\n';
            textBox2.Text += "Numero De Outputs: " + Outputnumber.ToString() + '\n';
            textBox2.Text += "Numero De Outputs arr: " + outputs.Count.ToString() + '\n';
            textBox2.Text += "Numero De Reglas: " + NumRules.ToString() + '\n';

            int[,] RuleArray = Reglas.ruleArray(NumRules,InputNumber,Outputnumber); 

            foreach (string s in Reglas.rulestring)
            {
                textBox2.Text += s +" ";
            }*/

            textBox2.Text = "#include <stdio.h>" + '\n';
            textBox2.Text += "#include \"funcionesFizz.h\"" + '\n';
            textBox2.Text += "int main(){" + '\n';
            for(int x=0;x<InputNumber;x++)
            {
                textBox2.Text += "double input" + x.ToString() + ";"+ '\n'; 
                textBox2.Text += "scanf(\"% lf\",&input" + x.ToString() + ");" + '\n';
            }



        }




        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = " All Files (*.*)|*.*";
            //openFileDialog1.FilterIndex = 1;
            try
            {
                DialogResult result = openFileDialog1.ShowDialog();
               // textBox1.Text = File.ReadAllText(openFileDialog1.FileName);
                StreamReader texto = new StreamReader(openFileDialog1.FileName);
                while (texto.Peek() >= 0)
                {
                    textBox1.Text += texto.ReadLine() + "|";
                }

            }
            catch (IOException) { textBox1.Text = "ERROR AL LEER"; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            processingText();
        }
    }
}
