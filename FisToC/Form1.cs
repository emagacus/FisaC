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
                    inp.name = lineas[index + 1].Replace("Name='","").Replace("'",""); 
                    string[] nmf = lineas[index + 3].Replace('=', ' ').Split(' '); //obtener el numero de funciones de membresia
                    inp.Nmf = Int32.Parse(nmf[1]);
                    int y = index+4;
                    for (int x = 0; x < inp.Nmf; x++)
                    {
                        string[] mfvalues = lineas[y].Replace('[', ' ').Replace(']', ' ').Split(' ');
                  
                        double[] v = new double [4];
                         v[0] = double.Parse(mfvalues[1]);
                         v[1] = double.Parse(mfvalues[2]);
                         v[2] = double.Parse(mfvalues[3]);
                         v[3] = double.Parse(mfvalues[4]);

                        var mf = new memfunction();
                        mf.range = v;
                        inp.mfs.Add(mf);
                        y++;

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
                        string[] mfo = lineas[index + 4 + x].Split('[');
                        string[] mfo1 = mfo[1].Replace(']',' ').Split(' ');
                       // Console.WriteLine("MF OUTPUT: " + mfo1[2]);
                        //Sacar el valor de la x en la funcion linear

                        // double val = mfo[];
                        ou.outval.Add(double.Parse(mfo1[2]));
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
                textBox2.Text += "printf(\"Ingrese Input para " + inputs[x].name +"\"); \n";
                textBox2.Text += "scanf(\"%lf\",&input" + x.ToString() + ");" + '\n';
            }

            int[,] arreglas = Reglas.ruleArray(NumRules, InputNumber, Outputnumber);
            int filas = arreglas.GetLength(0);
            int columnas = arreglas.GetLength(1);



            for (int i = 0; i < filas; i++)
            {
                string sgetval = "double v" + i.ToString() + "[] = {";
                int nmf = 0;

                for (int j = 0; j < columnas - 1; j++)
                {
                    if (arreglas[i, j] != 0)
                    {
                        textBox2.Text += "\n double a" + i.ToString() + j.ToString() + "[4] = {";
                        // textBox2.Text += "\n double r" + i.ToString() + "v" + j.ToString() + " = trapmf({";

                        double[] param1 = inputs[j].mfs[arreglas[i, j] - 1].range;
                        for (int x = 0; x < param1.GetLength(0) - 1; x++)
                        {
                            textBox2.Text += param1[x].ToString() + ",";
                        }
                        textBox2.Text += param1[param1.GetLength(0) - 1].ToString();
                    }
                    if (arreglas[i, j] != 0)
                    {
                        textBox2.Text += "};";
                        textBox2.Text += "\n double r" + i.ToString() + "v" + j.ToString() + " = trapmf(a" + i.ToString() + j.ToString() + ",input"+ j.ToString()+ ");";
                        sgetval += "r" + i.ToString() + "v" + j.ToString();
                        sgetval += ",";
                        nmf++;


                    }//Console.Write(string.Format("{0} ", arreglas[i, j]));
                }
                sgetval = sgetval.Remove(sgetval.Length - 1);
                sgetval += "};";
                textBox2.Text += "\n"+sgetval+"\n";                                                 //1 si es and 1 si es or
                string nO = Reglas.ruleOrAnd[i].ToString();
                textBox2.Text += "double res" +i.ToString() + " = getValue(" + "v" +i.ToString() + "," + nO + ","+ nmf.ToString() +");";
                //getvalue saca el valor de cada regla
            }


            //filas es tambien el numero de reglas

            textBox2.Text += "\n double ress[] = {";
            
            for (int x = 0; x < filas;x++)
            {
                textBox2.Text += "res" + x.ToString() + "*" + outputs[0].outval[Reglas.RuleOutput[x]-1];
                if (x != filas - 1) { textBox2.Text += ","; }
            }

           

            textBox2.Text += "};";

            textBox2.Text += "\n double mfres[] = {";

            for (int x = 0; x < filas; x++)
            {
                textBox2.Text += "res" + x.ToString();
                if (x != filas - 1) { textBox2.Text += ","; }
            }
            textBox2.Text += "};";
            textBox2.Text += "\n double resultado = takagiInf(ress,mfres,"+ filas.ToString() + ");";
            textBox2.Text += "\n printf(\"El resultado es %lf\",resultado);";
            textBox2.Text += "\n }";


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

    public class memfunction
    {
        public double[] range;
    }

    public class Input
    {
        public Input()
        {
            mfs = new List<memfunction>();
        }
        public int Nmf;
        public string name;
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

        public List<int> RuleOutput;
        public List<int> ruleOrAnd;
        public List<string> rulestring;
        public List<int> inputIndex;
        public List<int> OutputIndex;
        public Rules() { rulestring = new List<string>(); }

        public int[,] ruleArray(int nrules, int ninput, int noutput)
        {
            int columns = ninput + noutput;
            int[,] reglas = new int[nrules, columns];
            ruleOrAnd = new List<int>();
            RuleOutput = new List<int>();

            int filecount = 0;
            foreach (string s in rulestring)
            {
                //arreglo de reglas
                string[] array = s.Split(' ');
                for (int x = 0; x < columns; x++)
                {
                    array[x] = array[x].Trim(',');
                    
                    reglas[filecount, x] = int.Parse(array[x]);

                }

                //Aqui se saca si es OR o AND y el output que afecta
                //  Console.WriteLine("EL VALOR ACTUAL ES " + array[columns+2]);
                ruleOrAnd.Add(int.Parse(array[columns + 2]));
                RuleOutput.Add(int.Parse(array[columns - 1]));

                filecount++;
                if (filecount == nrules)
                {
                    
                    break;
                }
            }

            return reglas;
        }

    }

}
