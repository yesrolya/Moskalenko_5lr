using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moskalenko_5lr
{
    class Program
    {

        class LR5
        {
            int attr_q; //quantity of attributes
            int cl_q; //quantity of classes a, b, c...
            int[] cl_len; //classes length a1 a2 ... an
            int[,] matrix;
            List<int[]> M;
            List<List<int>> cluster;
            string logic;

            public LR5(int attribute_quantity, int classes_quantity, int[] classes_length, int[,] matrix)
            {
                //ширина матрицы =)
                this.attr_q = attribute_quantity;
                //количество классов: а, б, с...
                this.cl_q = classes_quantity;
                //количество подклассов в каждом и классов: а1, а2...
                this.cl_len = new int[classes_quantity];
                int sum = 0, k = 0;
                foreach (var c in classes_length)
                {
                    this.cl_len[k++] = c;
                    sum += c;
                }

                //исхлодная матрица признаков
                this.matrix = new int[sum, attribute_quantity];
                for (int i = 0; i < sum; i++)
                    for (int j = 0; j < sum; j++)
                        this.matrix[i, j] = matrix[i,j];
                CreateM(sum);
                DeleteFromM(FindMinQof1());
                PrintM();
                CreateLogic();
                MakeClusters();
            }

            private void CreateM(int sum_quantity) //на входе общее количество подклассов
            {
                int len = sum_quantity;
                int sum = 0;
                foreach (var c in cl_len)
                {
                    sum += (len - c) * c;
                    len -= c;
                }

                M = new List<int[]>();
                int this_class = 0, next_class = 0;
                for (int c_class = 0; c_class < cl_q; c_class++)
                {
                    //сложение по модулю 2
                    //признаков каждого подкласса с признаками подклассов других классов, без повторений
                    this_class = next_class; //начало индексов этого класса
                    next_class += cl_len[c_class]; //начало индексов следующего класса
                    for (int c_subclass = this_class; c_subclass < next_class && c_subclass < sum_quantity; c_subclass++)
                        for (int j = next_class; j < sum_quantity; j++)
                            M.Add(AddMod2(c_subclass, j));
                }
                PrintM();
            }

            private int[] AddMod2 (int first, int second)
            {
                int[] temp = new int[attr_q];
                // сложение по модулю 2 двух строк поэлементно 
                for (int j = 0; j < attr_q; j++)
                    temp[j] = (matrix[first, j] + matrix[second, j]) % 2;
                return temp;
            }

            private int FindMinQof1()
            {
                int minQ = attr_q;
                int minI = 0;
                for (int i = 0; i < M.Count(); i++)
                {
                    int Q = 0;
                    for (int j = 0; j < attr_q; j++)
                        if (M[i][j] == 1) Q++;
                    if (minQ >= Q)
                    {
                        minQ = Q;
                        minI = i;
                    }
                }
                Console.WriteLine("Min 1: " + minI);
                return minI;
            }

            private void PrintM ()
            {
                Console.WriteLine("Matrix M:");
                for (int i = 0; i < M.Count(); i++)
                {
                    for (int j = 0; j < attr_q; j++)
                        Console.Write(M[i][j] + " ");
                    Console.WriteLine();
                }
            }

            private void DeleteFromM(int index)
            {
                for (int i = M.Count() - 1; i >= 0; i--)
                {
                    //если все единицы в текущей строке совпадают с единицами в строке с индексом index, то она удаляется
                    bool deleteIt = true;
                    for (int j = 0; j < attr_q && deleteIt == true; j++)
                        if (M[index][j] == 1 && M[i][j] == 0)
                            deleteIt = false;
                    if (index != i && deleteIt)
                        M.RemoveAt(i);
                }
            }

            public void CreateLogic()
            {
                logic = "";
                string temp_logic = "";
                for (int i = 0; i < M.Count(); i++)
                {
                    temp_logic = "(";
                    for (int j = 0; j < attr_q; j++)
                        if (M[i][j] == 1)
                            temp_logic += "p" + j + "p" + "+";
                    temp_logic = temp_logic.Substring(0, temp_logic.Length - 1); //обрезаем последний + или (, если не было 1
                    if (temp_logic.Length != 0)
                    {
                        if (temp_logic.IndexOf('+') == -1)
                            temp_logic = temp_logic.Substring(1, temp_logic.Length - 1);
                        else
                            temp_logic += ")";
                        if (i != M.Count - 1)
                            temp_logic += "*";
                        logic += temp_logic;
                    }
                }

                if (logic[logic.Length - 1] == '*')
                    logic.Substring(0, logic.Length - 1);

                Console.WriteLine(logic);
            }

            private void MakeClusters()
            {
                List<List<int>> newM = new List<List<int>>();
                for (int i = 0; i < M.Count(); i++)
                {
                    var temp = new List<int>();
                    for (int j = 0; j < attr_q; j++)
                        if (M[i][j] == 1)
                            temp.Add(j);
                    if (temp.Count() != 0)
                        newM.Add(temp);
                }

                cluster = new List<List<int>>();
                //заполнение cluster
                Console.WriteLine("DDDDD");
                for (int j = 0; j < newM[0].Count; j++)
                {
                    func(newM, 0, j, "");
                }
            }

            //МАГИЯ 
            private void func (List<List<int>> N, int i, int j, string way)
            {
                if (i == N.Count)
                {
                    Console.WriteLine(way);
                    if (way[way.Length - 1] == ' ')
                        way = way.Substring(0, way.Length - 1);
                    var temp = new List<int>();
                    foreach (var w in way.Split(' '))
                        temp.Add(int.Parse(w));
                    cluster.Add(temp);
                } 
                else if (i + 1 < N.Count && i == 0)
                    for (int k = 0; k < N[i + 1].Count; k++)
                        func(N, i+1, k, way + N[i][j] + ' ');
                else if (way.IndexOf((N[i][j]).ToString()) != -1 && i < N.Count) {
                    if (i + 1 == N.Count)
                        func(N, i + 1, 0, way);
                    else
                        for (int k = 0; k < N[i + 1].Count; k++)
                            func(N, i + 1, k, way);
                }
                else 
                {
                    bool create = true;
                    for (int x = 0; x < i && create; x++)
                        for (int y = 0; y < N[x].Count && create; y++)
                            if (N[x][y] == N[i][j])
                                create = false;
                    string temp = way;
                    if (temp.Length != 0)
                    {
                        if (temp[temp.Length - 1] == ' ')
                            temp = temp.Substring(0, temp.Length - 1);

                        foreach (var w in temp.Split(' '))
                        {
                            int u = -1;
                            int.TryParse(w, out u);
                            if (N[i].IndexOf(u) != -1)
                            {
                                create = false;
                                break;
                            }
                        }
                    }
                    
                    if (create)
                        if (i + 1 == N.Count)
                            func(N, i + 1, 0, way + N[i][j]);
                        else
                            for (int k = 0; k < N[i + 1].Count; k++)
                                func(N, i + 1, k, way + N[i][j] + ' ');
                    else
                    {
                        string str = way + N[i][j];
                        for (int k = 0; i + 1 < N.Count && k < N[i + 1].Count; k++)
                            if (str.IndexOf(N[i+ 1][k].ToString()) != -1)
                                func(N, i + 1, k, str + ' ');
                    }
                }
            }


        }

        static void Main(string[] args)
        {
            int[,] matrix = {   { 0,1,0,0,1 },
                                { 1,1,0,1,0 },
                                { 0,1,1,0,1 },
                                { 1,0,1,0,1 },
                                { 1,0,0,1,1 } };
            int[] clq = { 2, 3 };
            LR5 lol = new LR5(5, 2, clq, matrix);
            Console.ReadKey();
        }
    }
}
