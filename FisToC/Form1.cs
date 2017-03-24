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


    public class memfunction
    {
        public int[] range;
    }

    public class Input
        {
        public Input()
        {
            mfs = new List<memfunction>();
        }
        public int Nmf;
        public List<memfunction> mfs;
        }

    public class Output
    {
        public Output()
        {
            outval = new List<double>();
        }

       public List<double> outval;
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
                    string[] nmf = lineas[index + 3].Replace('=', ' ').Split(' '); //obtener el numero de funciones de membresia
                    inp.Nmf = Int32.Parse(nmf[1]);
                    int y = index+4;
                    for (int x = 0; x < inp.Nmf; x++)
                    {
                        string[] mfvalues = lineas[y].Replace('[', ' ').Replace(']', ' ').Split(' ');
                        int[] v = new int [4];
                         v[0] = Int32.Parse(mfvalues[1]);
                         v[1] = Int32.Parse(mfvalues[2]);
                         v[2] = Int32.Parse(mfvalues[3]);
                         v[3] = Int32.Parse(mfvalues[4]);

                        var mf = new memfunction();
                        mf.range = v;
                        inp.mfs.Add(mf);
                        

                    }   //añadir funciones de mebresia

                    inputs.Add(inp);
                    InputNumber++;
                }
                if (s.Contains("Output" + (Outputnumber + 1).ToString()))
                {
                    Output ou = new Output();

                    string[] nmf = lineas[index + 3].Replace('=', ' ').Split(' '); //obtener el numero de funciones de membresia
                    int onmf = int.Parse(nmf[1]);

                    for (int x = 0; x < onmf; x++)
                    {
                        ou.outval.Add(0);
                    }

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
                textBox2.Text += "scanf(\"%lf\",&input" + x.ToString() + ");" + '\n';
            }

            int[,] arreglas = Reglas.ruleArray(NumRules, InputNumber, Outputnumber);
            int filas = arreglas.GetLength(0);
            int columnas = arreglas.GetLength(1);



            for (int i = 0; i < filas; i++)
            {
                string sgetval = "double v" + i.ToString() + "[] = {";

                for (int j = 0; j < columnas - 1; j++)
                {
                    if (arreglas[i, j] != 0)
                    {
                        textBox2.Text += "\n double a" + i.ToString() + j.ToString() + "[4] = {";
                        // textBox2.Text += "\n double r" + i.ToString() + "v" + j.ToString() + " = trapmf({";

                        int[] param1 = inputs[j].mfs[arreglas[i, j] - 1].range;
                        for (int x = 0; x < param1.GetLength(0) - 1; x++)
                        {
                            textBox2.Text += param1[x].ToString() + ",";
                        }
                        textBox2.Text += param1[param1.GetLength(0) - 1].ToString();
                    }
                    if (arreglas[i, j] != 0)
                    {
                        textBox2.Text += "};";
                        textBox2.Text += "\n double r" + i.ToString() + "v" + j.ToString() + " = trapmf(a" + i.ToString() + j.ToString() + ",1);";
                        sgetval += "r" + i.ToString() + "v" + j.ToString();
                        sgetval += ",";



                    }//Console.Write(string.Format("{0} ", arreglas[i, j]));
                }
                sgetval = sgetval.Remove(sgetval.Length - 1);
                sgetval += "};";
                textBox2.Text += "\n"+sgetval+"\n";
                textBox2.Text += "double res" +i.ToString() + " = getValue(" + "v" +i.ToString() + ",1);";
               
            }

            //filas es tambien el numero de reglas

            textBox2.Text += "\n double ress[] = {";
            
            for (int x = 0; x < filas;x++)
            {
                textBox2.Text += "res" + x.ToString();
                if (x != filas - 1) { textBox2.Text += ","; }
            }
            textBox2.Text += "};";
            textBox2.Text += "\n double resultado = takagiInf(ress);";
            textBox2.Text += "\n printf(el resultado es %lf,resultado);";



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
