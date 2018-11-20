using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Default.Interpreter.Calc
{
    public class CalculatorClient
    {
        public void Run()
        {
            while (true)
            {
                Console.Write(">");
                var exp = Console.ReadLine();

                var c = new CalculatorBuilder();
                c.Build(exp);
                var r = c.TreeNodes.interpret();

                Console.WriteLine(r);
                
                try
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } 
        }
    }
    /*以下构建树*/
    public class CalculatorBuilder
    {
        public Node TreeNodes;

        //通过表达式exp构建树TreeNodes
        public void Build(string exp) 
        { 
            //TrimBoth处理
            exp = exp.Trim();

            //Validation of not null
            if (String.IsNullOrEmpty(exp)) throw new ApplicationException("Format error");

            //标准化
            exp = exp.Replace("　", " ");
            exp = exp.Replace("（", "(");
            exp = exp.Replace("）", ")");
            //替换双字节空格
            while (exp.Contains(" ")) exp = exp.Replace(" ", ""); //清除空格

            this.TreeNodes = this.BuildNode(exp);
        }
        private Node BuildNode(String exp)
        {
            //删除两边无用的括号
            while (this.HasUseLessBracketBothEnd(exp)) exp = exp.Substring(1, exp.Length - 2);

            //如果为数字直接返回（不能包含负数，是因为负号与减号分不清）
            if (Regex.IsMatch(exp, @"^[\d.]+$")) return new ValueNode(Convert.ToDouble(exp));

            //通过找到运算符位置，确定数字一、数字二、运算符
            var index = this.LookOperatorIndex(exp, "+-");                //第一层次运算符
            if (index == -1) index = this.LookOperatorIndex(exp, "*/%");  //第二层次运算符
            if (index == -1) throw new ApplicationException("Format Error");
            var v1 = exp.Substring(0, index);
            if (String.IsNullOrEmpty(v1)) v1 = "0";//把负号统一换算为减号
            var v2 = exp.Substring(index + 1);
            var oper = exp.Substring(index, 1);

            //通过数字一、数字二、运算符形成树
            var me1 = this.BuildNode(v1);
            var me2 = this.BuildNode(v2);
            if (oper == "+") return new AddSymboleNode(me1, me2);
            else if (oper == "-") return new SubSymboleNode(me1, me2);
            else if (oper == "*") return new MulSymboleNode(me1, me2);
            else if (oper == "/") return new DivSymboleNode(me1, me2);
            else if (oper == "%") return new ModSymboleNode(me1, me2);

            throw new ApplicationException("Format Error");
        }
        //寻找表达式中的运算符(必须从后往前找)
        private int LookOperatorIndex(String exp, String opers)
        {
            var cs = exp.ToArray();
            var degree = 0;
            var cs_opers = opers.ToArray();
            for (int i = cs.Length - 1; i >= 0; i--)
            {
                var c = cs[i];
                if (c == ')') degree++;
                else if (c == '(') degree--;
                else if (degree == 0) if (cs_opers.Contains(c)) return i;
            }
            return -1;
        }
        private bool HasUseLessBracketBothEnd(String exp)
        {
            var cs = exp.ToArray();
            var degree = 0;
            for (int i = cs.Length - 1; i >= 0; i--)
            {
                var c = cs[i];
                if (c == ')') degree++;
                else if (c == '(') degree--;
                else if (degree == 0) return false;
            }
            return true;
        }
    }

    /*以下是节点类*/
    public interface Node { double interpret();}
    public class ValueNode : Node
    {
        public double Value;
        public ValueNode(double value) { this.Value = value; }
        public double interpret() { return this.Value; }
    }
    public abstract class SymbolNode : Node
    {
        protected Node Left;
        protected Node Right;
        public SymbolNode(Node left, Node right)
        {
            this.Left = left;
            this.Right = right;
        }
        public abstract double interpret();
    }
    public class AddSymboleNode : SymbolNode
    {
        public AddSymboleNode(Node left, Node right) : base(left, right) { }
        public override double interpret() { return Left.interpret() + Right.interpret(); }
    }
    public class SubSymboleNode : SymbolNode
    {
        public SubSymboleNode(Node left, Node right) : base(left, right) { }
        public override double interpret() { return Left.interpret() - Right.interpret(); }
    }
    public class MulSymboleNode : SymbolNode
    {
        public MulSymboleNode(Node left, Node right) : base(left, right) { }
        public override double interpret() { return Left.interpret() * Right.interpret(); }
    }
    public class DivSymboleNode : SymbolNode
    {
        public DivSymboleNode(Node left, Node right) : base(left, right) { }
        public override double interpret() { return Left.interpret() / Right.interpret(); }
    }
    public class ModSymboleNode : SymbolNode
    {
        public ModSymboleNode(Node left, Node right) : base(left, right) { }
        public override double interpret() { return Left.interpret() % Right.interpret(); }
    }
}
