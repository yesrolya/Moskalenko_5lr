/*
 * using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moskalenko_5lr
{
    public class ClassLogic
    {
        string logic;
        List<string> multipliers1;
        List<string> multipliers2;

        public ClassLogic(string logic_expression)
        {
            logic = logic_expression;
            multipliers1 = new List<string>();
            foreach (var s in logic.Split('*'))
            {
                if (s[0] == '(' && s[s.Length - 1] == ')')
                    multipliers1.Add(s.Substring(1, s.Length - 2));
                else
                    multipliers1.Add(s);
            }
        }
        
        string DeleteDoubleExpression(string s)
        {
            if (s[0] == '(' && s[s.Length - 1] == ')')
                s = s.Substring(1, s.Length - 1);
            var temp = s.Split('+');
            string result = "";
            foreach (var t in temp)
            {
                var t1 = t.Split();
                List<string> t2 = new List<string>();
                foreach (var tt in t1)
                    t2.Add(tt);
                
            }

            while (side.IndexOf("!!") != -1)
            {
                var pos = side.IndexOf("!!");
                side = side.Substring(0, pos) + side.Substring(pos + 2, side.Length - pos - 2);
            }
            return side;
        }

        void SplitSides(bool left)
        {
            //3) редактирование
            char splitter = (left ? '*' : '+');
            for (int count = 0; count < 5; count++)
            {
                string temp = "";
                //сначала делим строку по знаку
                string[] side = new string[(left ?
                    leftSide.Split(splitter, ',').Length :
                    rightSide.Split(splitter, ',').Length)];
                side = (left ?
                    leftSide.Split(splitter, ',') :
                    rightSide.Split(splitter, ','));

                //заменяем знаки запятыми
                for (int i = 0; i < side.Length; i++)
                {
                    int balance = 0;
                    if (side[i].IndexOf('(') == -1)
                    {
                        //соединяем строки без скобок
                        temp += side[i] + ',';
                    }
                    else
                    {
                        //для строк со скобками объединяем до достижения баланса
                        string t = side[i];
                        for (int j = 0; j < side[i].Length; j++)
                        {
                            if (side[i][j] == '(') balance += 1;
                            if (side[i][j] == ')') balance += -1;
                        }
                        while (balance != 0)
                        {
                            i++;
                            t += splitter + side[i];
                            for (int j = 0; j < side[i].Length; j++)
                            {
                                if (side[i][j] == '(') balance += 1;
                                if (side[i][j] == ')') balance += -1;
                            }
                        }

                        if (t[0] == '(' && t[t.Length - 1] == ')')
                            t = t.Substring(1, t.Length - 2);

                        temp += t + ',';
                    }
                }
                if (left)
                    leftSide = temp.Substring(0, temp.Length - 1);
                else
                    rightSide = temp.Substring(0, temp.Length - 1);
            }
        }

        void SortSides()
        {
            var sl = leftSide.Split(',');
            Array.Sort(sl);
            string temp = "";
            foreach (var s in sl)
            {
                temp += s + ',';
            }
            leftSide = temp.Substring(0, temp.Length - 1);

            var sr = rightSide.Split(',');
            Array.Sort(sr);
            temp = "";
            foreach (var s in sr)
            {
                temp += s + ',';
            }
            rightSide = temp.Substring(0, temp.Length - 1);
        }

        void CompareSides()
        {
            if (rightSide == leftSide)
                solved = true;
        }

        void NextStep()
        {
            if (solved) return;
            if (this.IsNot())
            {
                var temp = DeleteNot();
                var t = temp.Split('=');
                //Console.WriteLine(t[0] + '=' + t[1]);
                nodes.Add(new ClassLogic(t[0], t[1], output + "    ", this));
            }

            List<string> reduct = new List<string>();
            reduct = Reduction();
            if (reduct.Count() != 0)
                foreach (var r in reduct)
                {
                    if (!solved)
                    {
                        var t = r.Split('=');
                        if (t[0] != leftSide || t[1] != rightSide)
                            //Console.WriteLine(t[0] + '=' + t[1]);
                            nodes.Add(new ClassLogic(t[0], t[1], output + "    ", this));
                    }

                }
        }

        //поиск отрицаний
        bool IsNot()
        {
            foreach (var s in leftSide.Split(','))
            {
                if (s != "")
                    if (s[0] == '!' && ((s[1] == '(' && s[s.Length - 1] == ')') || (s.Length == 2)))
                        return true;
            }
            foreach (var s in rightSide.Split(','))
            {
                if (s != "")
                    if (s[0] == '!' && ((s[1] == '(' && s[s.Length - 1] == ')') || (s.Length == 2)))
                        return true;
            }

            return false;
        }

        List<string> Reduction()
        {
            string initial = leftSide + '=' + rightSide;

            List<string> solution = new List<string>();
            int pos1 = 0, pos2;
            int currentPos = leftSide.IndexOf('+');
            if (currentPos == -1)
            {
                currentPos = rightSide.IndexOf('*');
                if (currentPos != -1) currentPos += leftSide.Length + 1;
            }
            while (currentPos != -1)
            {
                pos1 = 0;
                pos2 = initial.Length - 1;
                char splitter = (currentPos <= leftSide.Length ? '+' : '*');
                //находим положение запятых вокруг первого '+'
                for (int k = currentPos; k > 0; k--)
                {
                    if (initial[k] == ',' || initial[k] == '=')
                    {
                        pos1 = k + 1;
                        break;
                    }
                }

                for (int k = currentPos; k < initial.Length; k++)
                {
                    if (initial[k] == ',' || initial[k] == '=')
                    {
                        pos2 = k - 1;
                        break;
                    }
                }

                //pos2 = initial.IndexOf(',', currentPos) - 1;
                //if (pos2 == -2) pos2 = initial.IndexOf('=', currentPos) - 1;
                //if (pos2 == -2) pos2 = initial.Length - 1;
                int balance = 0;
                string temp = "";

                foreach (string str in initial.Substring(pos1, pos2 - pos1 + 1).Split(splitter))
                {
                    if (str.IndexOf('(') != -1 || str.IndexOf(')') != -1 || balance != 0)
                    {
                        for (int y = 0; y < str.Length; y++)
                        {
                            if (str[y] == '(') balance += 1;
                            if (str[y] == ')') balance -= 1;
                        }
                        temp += str + splitter;
                    }
                    else
                    {
                        temp = str + ' ';
                    }
                    if (balance == 0)
                    {
                        solution.Add(
                        (initial.Substring(0, pos1)) +
                        temp.Substring(0, temp.Length - 1) +
                        initial.Substring(pos2 + 1, initial.Length - pos2 - 1)
                        );
                        temp = "";
                        balance = 0;
                    }
                }
                currentPos = initial.IndexOf('+', pos2);
                if (currentPos == -1 || currentPos > leftSide.Length) currentPos = initial.IndexOf('*', pos2);
            }


            return solution;
        }

        string DeleteNot()
        {
            //2) диаметральная инверсия
            var sl = leftSide.Split(',');
            var sr = rightSide.Split(',');
            List<string> left = new List<string>();
            List<string> right = new List<string>();
            foreach (var s in sl)
            {
                if (s[0] == '!' && ((s[1] == '(' && s[s.Length - 1] == ')') || (s.Length == 2)))
                {
                    string temp = s;
                    if (s[1] == '(' && s[s.Length - 1] == ')')
                    {
                        temp = s.Substring(2, s.Length - 3);
                    }
                    else if (s.Length == 2)
                    {
                        temp = s.Substring(1, s.Length - 1);
                    }
                    right.Add(temp);
                }
                else
                {
                    left.Add(s);
                }
            }

            foreach (var s in sr)
            {
                if (s[0] == '!' && ((s[1] == '(' && s[s.Length - 1] == ')') || (s.Length == 2)))
                {
                    string temp = s;
                    if (s[1] == '(' && s[s.Length - 1] == ')')
                    {
                        temp = s.Substring(2, s.Length - 3);
                    }
                    else if (s.IndexOf('+') == -1 && s.IndexOf('-') == -1 && s.IndexOf('*') == -1)
                    {
                        temp = s.Substring(1, s.Length - 1);
                    }
                    left.Add(temp);
                }
                else
                {
                    right.Add(s);
                }
            }
            string tLeft = "";
            if (left.Count() != 0) tLeft = left[0];
            string tRight = "";
            if (right.Count() != 0) tRight = right[0];


            for (int i = 1; i < left.Count(); i++)
                tLeft += ',' + left[i];

            for (int i = 1; i < right.Count(); i++)
                tRight += ',' + right[i];

            return tLeft + "=" + tRight;
        }
    }
}
*/