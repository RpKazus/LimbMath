using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LimbMath
{ 
    public class Limb
    {
        #region properties
        public List<Limb> Values = new List<Limb>();
        public string MathName = "1";
        public static bool DivideFractions = false;
        public Limb (Limb example)
        {
            MathName = example.MathName.ToString();
            Values.AddRange(example.Values);
        }
        public Limb ()
        {

        }
        public Limb (string mathValue)
        {
            MathName = mathValue;
        }
        public List<Limb> MainPart
        {
            get
            {
                double d = 0;
                List<Limb> Answer = new List<Limb>();
                if (MathName == "^")
                    Answer.Add(Values[0]);
                else if (MathName == "/")
                    Answer.Add(Values[0]);
                else if (MathName == "*")
                    foreach (Limb addit in Values)
                    {
                        if (!addit.IsNumber && !Double.TryParse(addit.Frac, out d))
                            Answer.Add(addit);
                    }
                else if (MathName == "+")
                    Answer.AddRange(Values);
                else if (!IsNumber && !Double.TryParse(MathName, out d)) Answer.Add(this);
                return Answer;
            }
        }
        public string Frac
        {
            get
            {
                decimal d = 0;
                string answer = MathName;
                if (MathName == "/")
                {
                    if (MainPart[0].IsNumber && Coefficient.Count == Values.Count - 1)
                    {
                        answer = (MainPart[0].Frac).ToString();
                        foreach (Limb limb in Values)
                            answer = Decimal.Divide(Convert.ToDecimal(answer), Convert.ToDecimal(limb.Frac)).ToString();
                    }
                }
                else if (decimal.TryParse(MathName, out d))
                    answer = MathName;
                return answer;
            }
        }
        public List<Limb> Coefficient
        {
            get
            {
                double d = 0;
                List<Limb> Answer = new List<Limb>();
                if (MathName == "^")
                    for (int i = 1; i < Values.Count; i++)
                        Answer.Add(Values[i]);
                /*else if (MathName == "+")
                {
                    for (int i = 0; i < Values.Count; i++)
                        if (Values[i].IsNumber || Double.TryParse(Values[i].Frac, out d))
                            Answer.Add(Values[i]);
                }*/
                else if (MathName == "/")
                    for (int i = 1; i < Values.Count; i++)
                    {
                        if (Values[i].IsNumber)
                            Answer.Add(new Limb() { MathName = "/", Values = new List<Limb>() { new Limb() { MathName = "1" }, Values[i] } });
                    }
                else if (MathName == "*")
                {
                    foreach (Limb addit in Values)
                        if (addit.IsNumber || Double.TryParse(addit.Frac, out d))
                            Answer.Add(addit);
                }
                else if (IsNumber || Double.TryParse(Frac, out d))
                    Answer.Add(this);
                return Answer;
            }
            set
            {
                foreach(Limb limb1 in Coefficient)
                {
                    Values.Remove(limb1);
                }
                Values.AddRange(value);
            }
        }
        public bool IsNumber
        {
            get
            {
                decimal d;
                return decimal.TryParse(Frac.Replace('.', ','), out d);
            }
        }
        #endregion
        #region methods
        public Limb ToUsefulFrac()
        {
            Limb limb = new Limb(this);
            if(MathName == "/" && Coefficient.Count == 1 && !MainPart[0].IsNumber)
            {
                limb.MathName = "*";
                limb.Coefficient = new List<Limb>();
                foreach (Limb tLimb in Coefficient)
                    if (tLimb.IsNumber)
                        limb.Values.Add(tLimb);
                    else
                    {
                        limb = new Limb(this);
                        return limb;
                    }
            }
            return limb;
        }
        public override int GetHashCode()
        {
            int result = MathName.GetHashCode();
            foreach (Limb lmb in Values)
                result += lmb.GetHashCode();
            return result;
        }
        public void Reverse()
        {
            if (MathName == "+")
            {
                foreach (Limb lmb in Coefficient)
                {
                    decimal d;
                    if (lmb.Values.Count == 0)
                        if (Decimal.TryParse(lmb.MathName, out d))
                            lmb.MathName = (Convert.ToDecimal(lmb.MathName) * -1).ToString();
                        else
                            lmb.Values[0].Reverse();
                }
            }
            else if (MathName == "*" || MathName == "/")
                if (Coefficient.Count >= 1)
                    Coefficient[0].Reverse();
                else
                    Values.Add(new Limb() { MathName = "-1" });
            else if (MathName == "^")
                if (Values[0].IsNumber)
                    Values.Reverse();
                else
                {
                    Values = new List<Limb>()
                    {
                        new Limb() {MathName = "-1"},
                        new Limb(this)
                    };
                    MathName = "*";
                }
            else if (IsNumber)
            {
                Limb end = (new Fraction(this) * new Fraction(-1)).ToLimb();
                MathName = end.MathName;
                Values = end.Values;
            }
        }
        public override string ToString()
        {
            string answer = "";
            if (PriorSet(MathName) != -1)
            {
                IEnumerable<Limb> limbsOrdered = Values;
                if (MathName == "*")
                        limbsOrdered = Values.ToArray().OrderByDescending(q => Convert.ToInt16(q.IsNumber));
                for (int i = 0; i < limbsOrdered.ToArray().Count(); i++)
                {
                    string str = limbsOrdered.ToArray()[i].ToString();
                    answer += (i > 0 ? MathName : "") + (((PriorSet(limbsOrdered.ToArray()[i].MathName) != -1 && PriorSet(MathName) < PriorSet(limbsOrdered.ToArray()[i].MathName)) || ((str[0] != '-' || MathName == "+" || i == 0) && PriorSet(limbsOrdered.ToArray()[i].MathName) == -1)) ? str : "(" + str + ")");
                    answer = answer.Replace("++", "+");
                    answer = answer.Replace("+-", "-");
                    answer = answer.Replace("--", "+");
                    answer = answer.Replace("-+", "-");
                    answer = answer.Replace("-1*", "-");
                    answer = answer.Replace("+(-", "-(");
                }
            }
            else return MathName;
            return answer;
        }
        public override bool Equals(object obj)
        {
            Limb limb = (Limb)obj;
            if (PriorSet(limb.MathName) == -1 && limb.MathName == MathName)
                return true;
            else if (limb.MathName == MathName && PriorSet(limb.MathName) != -1)
            {
                foreach (Limb tLimb in (limb.MainPart.Count >= MainPart.Count) ? limb.MainPart : MainPart)
                {
                    bool IsExist = false;
                    foreach (Limb mLimb in (limb.MainPart.Count >= MainPart.Count) ? MainPart : limb.MainPart)
                        if (mLimb.Equals(tLimb))
                            IsExist = true;
                    if (!IsExist)
                        return false;
                }
                return true;
            }
            else return false;
        }
        public bool EqualsTo(object obj, string MinName)
        {
            Limb limb = (Limb)obj;
            short myPr = PriorSet(MathName);
            short tPr = PriorSet(limb.MathName);
            if (myPr == tPr)
            {
                if (Equals(limb))
                    return true;
                return false;
            }
            else if (((tPr == 2 && MinName == "+") || (tPr == 4 && MinName == "*")) && limb.MainPart.Count > 0)
            {
                foreach (Limb lmb in limb.MainPart)
                    if (!limb.Equals(lmb))
                        return false;
                return true;
            }
            else if (((myPr == 2 && MinName == "+") || (myPr == 4 && MinName == "*")) && MainPart.Count > 0)
            {
                foreach (Limb lmb in MainPart)
                    if (!lmb.Equals(limb))
                        return false;
                return true;
            }
            else return false;
        }
        #endregion
        #region mainMethods
        public Limb OpenBrackets()
        {
            for(int i = 0; i < Values.Count; i++)
                if(PriorSet(MathName) > PriorSet(Values[i].MathName) && PriorSet(Values[i].MathName) != -1)
                {
                    Limb root = new Limb(Values[i].MathName);
                    for (int minStep = 0; minStep < Values[i].Values.Count; minStep++)
                    {
                        Limb limb1 = new Limb(MathName);
                        if (PriorSet(MathName) < PriorSet(Values[i].Values[minStep].MathName) || PriorSet(Values[i].Values[minStep].MathName) == -1)
                            limb1.Values.Add(Values[i].Values[minStep]);
                        else if (MathName == Values[i].Values[minStep].MathName)
                            limb1.Values.AddRange(Values[i].Values[minStep].Values);
                        else if (PriorSet(MathName) > PriorSet(Values[i].Values[minStep].MathName))
                            limb1.Values.Add(Values[i].Values[minStep].OpenBrackets());
                        for (int maxStep = 0; maxStep < Values.Count; maxStep++)
                            if(i != maxStep)
                            {
                                if (PriorSet(MathName) < PriorSet(Values[maxStep].MathName) || PriorSet(Values[maxStep].MathName) == -1)
                                    limb1.Values.Add(Values[maxStep]);
                                else if (MathName == Values[maxStep].MathName)
                                    limb1.Values.AddRange(Values[maxStep].Values);
                                else if (PriorSet(MathName) > PriorSet(Values[maxStep].MathName))
                                    limb1.Values.Add(Values[maxStep].OpenBrackets());
                            }
                        limb1 = limb1.OpenBrackets();
                        if (PriorSet(root.MathName) < PriorSet(limb1.MathName) || PriorSet(limb1.MathName) == -1)
                            root.Values.Add(limb1);
                        else if (root.MathName == limb1.MathName)
                            root.Values.AddRange(limb1.Values);
                        else if (PriorSet(root.MathName) > PriorSet(limb1.MathName))
                            root.Values.Add(limb1.OpenBrackets());
                    }
                    root = root.Simpler();
                    return root;
                }
            return Simpler();
        }
        public Limb Simpler()
        {
            DateTime date = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("     Begin: " + (date - date));
            Console.ForegroundColor = ConsoleColor.Gray;
            Limb simpleLimb = this;
            simpleLimb.Values = simpleLimb.Values.OrderBy(p => p.GetHashCode()).ToList();
            if (simpleLimb.Values.Count >= 2)
            {
                for (int i = 0; i < simpleLimb.Values.Count; i++)
                {
                    for (int i2 = 0; i2 < simpleLimb.Values.Count; i2++)
                    {
                        if (i != i2)
                        {
                            Limb limb1 = simpleLimb.Values[i].ToUsefulFrac();
                            if (limb1.Values.Count >= 2)
                                limb1 = limb1.OpenBrackets();
                            Limb limb2 = simpleLimb.Values[i2].ToUsefulFrac();
                            if (limb2.Values.Count >= 2)
                                limb2 = limb2.OpenBrackets();
                            if (limb1.EqualsTo(limb2, MathName) && !limb1.IsNumber)
                            {
                                simpleLimb.Values.RemoveAt(i2);
                                if (i >= i2) i--;
                                i2--;
                                Limb coeff = new Limb();
                                coeff.MathName = "+";
                                coeff.Values.AddRange((limb2.Coefficient.Count >= 1) ? limb2.Coefficient : new List<Limb>() { new Limb() { MathName = "1" } });
                                if (MathName == "/") coeff.Values[0].Reverse();
                                coeff.Values.AddRange((limb1.Coefficient.Count >= 1) ? limb1.Coefficient : new List<Limb>() { new Limb() { MathName = "1" } });
                                coeff = coeff.Simpler();
                                if (MathName == "+")
                                {
                                    Limb temp = new Limb("*");
                                    temp.Values.Add(coeff);
                                    if (limb1.MathName == temp.MathName)
                                        temp.Values.AddRange(limb1.Values);
                                    else temp.Values.Add(new Limb() { MathName = limb1.MathName, Values = limb1.Values, Coefficient = new List<Limb>() });
                                    simpleLimb.Values[i] = temp;
                                }
                                else if (MathName == "*" || MathName == "/")
                                {
                                    Limb temp = new Limb("^");
                                    if (limb1.MathName == temp.MathName)
                                        temp.Values.AddRange(limb1.Values);
                                    else temp.Values.Add(new Limb() { MathName = limb1.MathName, Values = limb1.Values, Coefficient = new List<Limb>() });
                                    temp.Values.Add(coeff);
                                    simpleLimb.Values[i] = temp;
                                }
                                if (MathName == "+")
                                    if (PriorSet(simpleLimb.Values[i].Values[1].MathName) != -1 && simpleLimb.Values[i].Values[1].Values.Count == 1)
                                        simpleLimb.Values[i].Values[1] = simpleLimb.Values[i].Values[1].Values[0];
                                    else if (MathName == "*" || MathName == "/")
                                        if (PriorSet(simpleLimb.Values[i].Values[0].MathName) != -1 && simpleLimb.Values[i].Values[0].Values.Count == 1)
                                            simpleLimb.Values[i].Values[0] = simpleLimb.Values[i].Values[0].Values[0];
                            }
                            else if (limb2.IsNumber && limb1.IsNumber && !(simpleLimb.MathName == "/" && !DivideFractions))
                            {
                                simpleLimb.Values.RemoveAt(i2);
                                if (i >= i2) i--;
                                i2--;
                                if (simpleLimb.MathName == "+")
                                    limb1 = (new Fraction(limb1) + new Fraction(limb2)).ToLimb();
                                else if (simpleLimb.MathName == "*")
                                    limb1 = (new Fraction(limb1) * new Fraction(limb2)).ToLimb();
                                else if (DivideFractions && simpleLimb.MathName == "/" && i == 0)
                                    limb1 = (new Fraction(limb1) / new Fraction(limb2)).ToLimb();
                                else if (DivideFractions && simpleLimb.MathName == "/" && i > 0)
                                    limb1 = (new Fraction(limb1) * new Fraction(limb2)).ToLimb();
                                else if (simpleLimb.MathName == "^" && i > 0)
                                    limb1 = (new Fraction(limb1) * new Fraction(limb2)).ToLimb();
                                else if (simpleLimb.MathName == "^" && i == 0)
                                    limb1 = (new Fraction(limb1) ^ new Fraction(limb2)).ToLimb();
                                simpleLimb.Values[i] = limb1;
                            }
                            else
                            {
                                simpleLimb.Values[i] = limb1;
                                simpleLimb.Values[i2] = limb2; 
                            }
                        }
                    }
                }
            }
            else if (simpleLimb.MainPart.Count == 0 && PriorSet(simpleLimb.MathName) != -1)
                simpleLimb.MathName = "1";
            if (DateTime.Now - date >= new TimeSpan(0, 0, 1))
                Console.ForegroundColor = ConsoleColor.White;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("     For End: " + (DateTime.Now - date) + "; " + simpleLimb);
            Console.ForegroundColor = ConsoleColor.Red;
            date = DateTime.Now;
            if (simpleLimb.Values.Count == 1 && PriorSet(simpleLimb.MathName) != -1)
                simpleLimb = simpleLimb.Values[0];
            Console.WriteLine("     Methods End: " + (DateTime.Now - date));
            date = DateTime.Now;
            if (simpleLimb.Coefficient.Count == 1 && simpleLimb.MainPart.Count >= 1)
                if (simpleLimb.MathName == "*" || simpleLimb.MathName == "^" || simpleLimb.MathName == "/")
                {
                    if (simpleLimb.Coefficient[0].MathName == "1")
                        if (simpleLimb.MainPart.Count >= 2)
                            simpleLimb.Coefficient = new List<Limb>();
                        else
                            simpleLimb = simpleLimb.MainPart[0];
                    else if (simpleLimb.Coefficient[0].MathName == "0")
                        simpleLimb = new Limb() { MathName = simpleLimb.MathName == "^" ? "1" : "0" };
                }             
                else if (simpleLimb.MathName == "+")
                {
                    foreach (Limb limb in simpleLimb.Values)
                        if (limb.MathName == "0")
                            if (simpleLimb.Values.Count >= 2)
                                simpleLimb.Values.Remove(limb);
                            else
                                simpleLimb = simpleLimb.MainPart[0];
                }
                
            Console.WriteLine("     End: " + (DateTime.Now - date));
            Console.ForegroundColor = ConsoleColor.Gray;
            return simpleLimb;
        }
        public static Int16 PriorSet(string str)
        {
            switch(str)
            {
                case "=":
                    return 0;
                case "+":
                    return 1;
                case "-":
                    return 1;
                case "*":
                    return 2;
                case "/":
                    return 3;
                case "^":
                    return 4;
                default:
                    return -1;
            }
        }
        public static short FindChar(string str, short level)
        {
            short curlevel = 4;
            short pos = -1;
            for(short i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    i = (short)(FindBracket(str, i, true) + 1);
                    if (i >= str.Length)
                        return pos;
                }
                else if (str[i] == ')')
                    return pos;
                short temp = PriorSet(str[i].ToString());
                if (i != 0 && temp >= level && temp <= curlevel)
                {
                    curlevel = temp;
                    pos = i;
                }
            }
            return pos;
        }
        public static List<short> FindMassiveOfChars(short mathValue, string str, short level)
        {
            List<short> values = new List<short>();
            for (short i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    i = (short)(FindBracket(str, i, true) + 1);
                    if (i >= str.Length)
                        return values;
                }
                else if (str[i] == ')')
                    return values;
                if (PriorSet(str[i].ToString()) != -1 && PriorSet(str[i].ToString()) <= mathValue)
                    values.Add(i);
            }
            return values;
        }
        public static Limb ConvertTextToLimb(string str, short level)
        {
            if (str[0] == '+')
                str = str.Substring(1);
            if (str[0] == '(')
                if (FindBracket(str, 0, true) == str.Length - 1)
                    str = str.Substring(1, str.Length - 2);
            if (PriorSet(str[0].ToString()) != 1)
                str = "=" + str;

            Limb limb = new Limb();
            List<string> values = new List<string>();
                str = FindCurrLimb(str);
                short temp = FindChar(str, 0);
            if (temp != -1)
            {
                limb.MathName = str[temp].ToString();
                if (limb.MathName == "-")
                    limb.MathName = "+";
                foreach (short curr in FindMassiveOfChars(PriorSet(limb.MathName), str, 0))
                {
                    Limb tLimb = ConvertTextToLimb(FindLimb(str, curr, PriorSet(limb.MathName), true), PriorSet(limb.MathName));
                    if (tLimb.MathName == limb.MathName)
                        limb.Values.AddRange(tLimb.Values);
                    else
                        limb.Values.Add(tLimb);
                }
            }
            else
            {
                limb.MathName = str;
                if (!limb.IsNumber)
                    limb = new Limb() {MathName = (str[0] == '=') ? str.Substring(1) : str };
            }
            if (limb.MathName.Length > 1 && limb.MathName[0] == '=')
                limb.MathName = limb.MathName.Substring(1);
            return limb;
        }
        public static Int16 FindBracket(string str, short point,bool direction)
        {
            int step = 0;
            if(direction)
                for (short i = point; i < str.Length; i++)
                {
                    if (str[i] == '(')
                        step++;
                    else if (str[i] == ')')
                        if (step > 1)
                            step--;
                        else
                            return i;
                    if (i >= str.Length - 1)
                        throw new Exception("Отсутствие скобки закрытия");
                }
            #region else
            else
                for (short i = point; i >= 0; i--)
                {
                    if (str[i] == ')')
                        step++;
                    else if (str[i] == '(')
                        if (step > 1)
                            step--;
                        else
                            return i;
                    if (i <= 0)
                        throw new Exception("Отсутствие скобки открытия");
                }
            return -1;
            #endregion
        }
        public static string FindLimb(string str, Int16 point, short level, bool direction)
        {
            if(direction)
            for(Int16 i = point; i < str.Length; i++)
                if (i < str.Length - 1)
                {
                        if (str[i + 1] == '(')
                        {
                            i = FindBracket(str, (short)(i + 1), true);
                            if (i >= str.Length - 1)
                                return str.Substring(str[point] == '-'? point : point + 1);
                        }
                        else if (str[i + 1] == ')') return str.Substring(str[point] == '-' ? point : point + 1, str[point] == '-' ? i - point + 1 : i - point);
                        if (PriorSet(str[i + 1].ToString()) != -1 && PriorSet(str[i + 1].ToString()) <= level)
                            return str.Substring(str[point] == '-' ? point : point + 1, str[point] == '-' ? i - point + 1 : i - point);
                }
                    else return str.Substring(str[point] == '-' ? point : point + 1, str[point] == '-' ? i - point + 1 : i - point);
            return String.Empty;
        }
        public static string FindCurrLimb(string str)
        {
            string mathType = "";
            for(short i = 0; i < str.Length; i++)
            {
                if (mathType == "")
                    if (str[i] == '+' || str[i] == '-' || Char.IsNumber(str[i]))
                        mathType = "num";
                    else if (Char.IsLetter(str[i]))
                        mathType = "var";
                if (str[i] == '(')
                {
                    i = FindBracket(str, i, true);
                    if (i + 1 < str.Length)
                        if (str[i + 1] != ')' && PriorSet(str[i + 1].ToString()) == -1)
                            str = str.Substring(0, i + 1) + "*" + str.Substring(i + 1, str.Length - i - 1);
                }
                if (i + 1 < str.Length)
                {
                    if (str[i] == '-' && !Char.IsNumber(str[i + 1]))
                    {
                        str = str.Substring(0, i + 1) + "1*" + str.Substring(i + 1, str.Length - i - 1);
                        i++;
                    }
                    if (Char.IsLetter(str[i + 1]) && PriorSet(str[i].ToString()) == -1)
                        str = str.Substring(0, i + 1) + "*" + str.Substring(i + 1, str.Length - i - 1);
                    if (Char.IsLetter(str[i]) && str[i + 1] == '(')
                        str = str.Substring(0, i + 1) + "*" + str.Substring(i + 1, str.Length - i - 1);
                }
            }
            return str;
        }
        #endregion
        #region Math
        
        #endregion
    }
}
