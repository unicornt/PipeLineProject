using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity;
using System.Windows.Forms;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public static class Var
{
    public static ulong[] code = new ulong[233333];
    public static string[] ins = new string[2500];
}
namespace load
{
    //FILE* fp;
    class Work {
        static bool Isdigit(char x) {
            if (x >= 48 && x <= 57) return true;
            return false;
        }
        static bool Isalpha(char c) {
            if (c >= 'a' && c <= 'f') return true;
            return false;
        }
        static bool Ishex(char c) {
            return Isdigit(c) || Isalpha(c);
        }
        static ulong Val(char x)
        {
            if (Isdigit(x)) return (ulong)(x - '0');
            return (ulong)(x - 'a' + 10);
        }
        static void Rd(string filepath)
        {
            if (filepath == "")
            {
                UnityEngine.UI.Button errorButton = GameObject.Find("UICanvas/ButtonPanel/ErrorButton").GetComponent<Button>();
                errorButton.onClick.Invoke();
            }
            using (StreamReader fp = new StreamReader(filepath))
            {
                string str;
                while ((str = fp.ReadLine()) != null)
                {
                    if (str[1] == 'x')
                    {
                        int i = 2;
                        ulong st = 0;
                        for (; str[i] != ':'; ++i)
                            st = st << 4 | Val(str[i]);
                        for (++i;; ++i)
                        {
                            if (str[i] == '|') break;
                            if (!Ishex(str[i])) continue;
                            Var.code[st++] = Val(str[i]) << 4 | Val(str[i + 1]);
                            ++i;
                        }
                        string newstring = str.Substring(i + 1);
                        int len = newstring.Length;
                        Var.ins[st] = "";
                        for (i = 0; i < len; ++i)
                        {
                            if (Var.ins[st].Length == 0 && newstring[i] == ' ') continue;
                            if (newstring[i] == '#') break;
                            Var.ins[st] = Var.ins[st] + newstring[i];
                            if (newstring[i] == ':') Var.ins[st] = "";
                        }
                        
                    }
                }
            }
        }
        public static void Gao(string filepath)
        {
            //puts("Please input the input file");
            //string s;
            //scanf("%s", str);
            //s = Console.ReadLine();
            Rd(filepath);
        }
    }
}
struct Prece
{
    public bool ZF, SF, OF, set_cc, stall, bubble;
    public ulong stat, icode, ifun, rA, rB, dstE, dstM, srcA, srcB;
    public ulong valA, valB, valC, valE, valM, valP;
    public ulong pc;
    public void Clear()
    {
        set_cc = stall = bubble = false;
        stat = icode = 1;
        ifun = 0;
        rA = rB = dstE = dstM = srcA = srcB = 0xf;
        valA = valB = valC = valE = valM = valP = 0;
        pc = 23333;
    }
    public void SetBubble()
    {
        Clear();
        bubble = true;
    }
};
namespace conduct {
    class Work
    {
        public static ulong[] reg = new ulong[16];
        public static ulong[] mem = new ulong[100005];
        public static Prece D, E, M, W;
        public static Prece f, d, e, m;
        public static bool F_stall, ZF, SF, OF;
        public static ulong predPC, pc;
        public static ulong stat;
        public static int cycle;
        static ulong GetVal(ulong st, ulong bitlen)
        {
            ulong res = 0;
            for (ulong i = st + bitlen - 1; i >= st; --i)
                res = res << 8 | Var.code[i];
            return res;
        }
        static bool Cnd(Prece x, ulong fun)
        {
            switch (fun)
            {
                case (0): return true;
                case (1): return (x.SF ^ x.OF) | x.ZF;
                case (2): return x.SF ^ x.OF;
                case (3): return x.ZF;
                case (4): return !x.ZF;
                case (5): return !(x.SF ^ x.OF);
                case (6): return !(x.SF ^ x.OF) & !x.ZF;
                default: return false;
            }
        }
        static void Chk(bool opt, ref ulong dst, ulong val)
        {
            if (!opt) dst = val;
        }
        static ulong Alu(ulong a, ulong b, ulong fun)
        {
            if (fun == 0) return b + a;
            if (fun == 1) return b + (~a) + 1;
            if (fun == 2) return b & a;
            if (fun == 3) return b ^ a;
            return 0;
        }
        static ulong Mem_sys_read(ulong addr)
        {
            return mem[addr >> 3];
        }
        static void Mem_sys_write(ulong addr, ulong val)
        {
            mem[addr >> 3] = val;
        }
        static void Mem_sys_load()
        {
            for (int i = 0; i < 23333; i += 8)
                for (int j = i + 7; j >= i; --j)
                    mem[i >> 3] = mem[i >> 3] << 8 | Var.code[j];
        }

        public static void Init()
        {
            stat = 1;
            ZF = true;
            reg[4] = 800000;
            f.Clear();
            d.Clear();
            e.Clear();
            m.Clear();
            D.Clear();
            E.Clear();
            M.Clear();
            W.Clear();// nop
            predPC = 0;
            Mem_sys_load();
            cycle = 0;
            OutputFetch();
            OutputDecode();
            OutputExecute();
            OutputMemory();
            OutputWriteback();
        }
        static void PosEdge()
        {
            if (W.dstE != 0xf) reg[W.dstE] = W.valE;
            if (W.dstM != 0xf) reg[W.dstM] = W.valM;
            W = m; M = e; E = d;
            if (!D.stall) D = f;
            stat = W.stat;
        }

        public static void ClickRun()
        {//run once
            ++cycle;//round
            PosEdge();
            Writeback();
            Memory();
            Execute();
            Decode();
            Fetch();
        }
        static void TotalRun()
        {//run until end
            for (; stat == 1;) ClickRun();
        }
        static void StepRun(int step)
        {//run exactly step times
            for (int i = 0; i < step; ++i)
                ClickRun();
        }
        static void OutputReg()
        {

        }

        static string GetCode(ulong pc, bool bubble)
        {
            if (bubble) return "Bubble";
            else if (pc == 23333 || Var.ins[pc] == null) return "";
            else return Var.ins[pc];
        }
        static void OutputFetch() {
            ClickToChangeValue.ChangeValue(0, "predPC", predPC.ToString());
            ClickToChangeValue.ChangeCode(0, GetCode(pc, false));
        }
        static void Fetch() {

            F_stall = false;

            if (W.icode == 9) pc = W.valM;
            //ret command in Writeback stage
            else if (M.icode == 7 && !Cnd(M, M.ifun)) pc = M.valA;
            //jxx command in memory but not jump
            else
            {
                pc = predPC;
                if (D.icode == 9 || E.icode == 9 || M.icode == 9) F_stall = true;//ret
                if (E.icode == 0xb || E.icode == 5) //popq or rmmovq
                    if (D.rA != 0xf && D.rA == E.dstM || D.rB != 0xf && D.rB == E.dstM) F_stall = true;
                if (F_stall == true)
                {
                    OutputFetch();//stall
                    return;
                }
            }
            //Select PC
            /////////////////////////////////////////////////////

            f.Clear();

            if (pc >= 23333)
            {
                f.stat = 3;
                OutputFetch();
                return;
            }

            f.icode = Var.code[pc] >> 4;
            f.ifun = Var.code[pc] & 0xf;
            f.stat = (f.icode == 0ul) ? 2ul : 1ul;
            f.pc = pc;

            switch (f.icode)
            {
                case (0): predPC = f.valP = pc + 1; break; // hlt
                case (1): predPC = f.valP = pc + 1; break; // nop
                case (2): //rrmovq or cmovXX rA rB
                    predPC = f.valP = pc + 2;
                    f.rA = Var.code[pc + 1] >> 4;
                    f.rB = Var.code[pc + 1] & 0xf;
                    Chk(f.rA != 0xf, ref f.stat, 4); // assert(rA != 0xf)
                    Chk(f.rB != 0xf, ref f.stat, 4);
                    break;
                case (3): //irmovq valC rB
                    predPC = f.valP = pc + 10;
                    f.rA = Var.code[pc + 1] >> 4;
                    f.rB = Var.code[pc + 1] & 0xf;
                    f.valC = GetVal(pc + 2, 8);
                    Chk(f.rA == 0xf, ref f.stat, 4); // assert(rA == 0xf)
                    Chk(f.rB != 0xf, ref f.stat, 4); // assert(rB != 0xf)
                    break;
                case (4): //rmmovq rA, D(rB)
                    predPC = f.valP = pc + 10;
                    f.rA = Var.code[pc + 1] >> 4;
                    f.rB = Var.code[pc + 1] & 0xf;
                    f.valC = GetVal(pc + 2, 8);
                    Chk(f.rA != 0xf, ref f.stat, 4);// assert(rA != 0xf)
                                                //chk(f.rB != 0xf, f.stat, 4);// assert(rB != 0xf)
                    break;
                case (5): //mrmovq D(rB), rA
                    predPC = f.valP = pc + 10;
                    f.rA = Var.code[pc + 1] >> 4;
                    f.rB = Var.code[pc + 1] & 0xf;
                    f.valC = GetVal(pc + 2, 8);
                    Chk(f.rA != 0xf, ref f.stat, 4);// assert(rA != 0xf)
                                                //chk(f.rB != 0xf, f.stat, 4);// assert(rB != 0xf)
                    break;
                case (6): //OPq rA, rB
                    predPC = f.valP = pc + 2;
                    f.rA = Var.code[pc + 1] >> 4;
                    f.rB = Var.code[pc + 1] & 0xf;
                    Chk(f.rA != 0xf, ref f.stat, 4);// assert(rA != 0xf)
                    Chk(f.rB != 0xf, ref f.stat, 4);// assert(rB != 0xf)
                    break;
                case (7): //jxx valC(Dest)
                    predPC = f.valC = GetVal(pc + 1, 8);
                    f.valP = pc + 9;
                    break;
                case (8): //call valC(Dest)
                    predPC = f.valC = GetVal(pc + 1, 8);
                    f.valP = pc + 9;
                    break;
                case (9): //ret
                    predPC = f.valP = pc + 1;
                    break;
                case (0xa): //pushq rA
                    predPC = f.valP = pc + 2;
                    f.rA = Var.code[pc + 1] >> 4;
                    f.rB = Var.code[pc + 1] & 0xf;
                    Chk(f.rA != 0xf, ref f.stat, 4);// assert(rA != 0xf)
                    Chk(f.rB == 0xf, ref f.stat, 4);// assert(rB == 0xf)
                    break;
                case (0xb): //popq rA
                    predPC = f.valP = pc + 2;
                    f.rA = Var.code[pc + 1] >> 4;
                    f.rB = Var.code[pc + 1] & 0xf;
                    Chk(f.rA != 0xf, ref f.stat, 4);// assert(rA != 0xf)
                    Chk(f.rB == 0xf, ref f.stat, 4);// assert(rB == 0xf)
                    break;
                default: f.stat = 4; break;//INS
            }//Predict PC
            if (f.stat != 1)
            {
                ulong _stat = f.stat;
                f.Clear();
                f.stat = _stat;
            }
            OutputFetch();

        }
        static void OutputDecode() {
            ClickToChangeValue.ChangeValue(1, "stat", D.stat.ToString());
            ClickToChangeValue.ChangeValue(1, "icode", D.icode.ToString());
            ClickToChangeValue.ChangeValue(1, "ifun", D.ifun.ToString());
            ClickToChangeValue.ChangeValue(1, "rA", D.rA.ToString());
            ClickToChangeValue.ChangeValue(1, "rB", D.rB.ToString());
            ClickToChangeValue.ChangeValue(1, "valC", D.valC.ToString());
            ClickToChangeValue.ChangeValue(1, "valP", D.valP.ToString());
            ClickToChangeValue.ChangeCode(1, GetCode(D.pc, D.bubble));
        }
        static void Decode()
        {
            D.stall = false;

            if (E.icode == 0xb || E.icode == 5) //popq or rmmovq
                if (D.rA != 0xf && D.rA == E.dstM || D.rB != 0xf && D.rB == E.dstM) D.stall = true;
            if (D.stall) {
                OutputDecode();
                return;
            }   
            //check whether stall


            if (M.icode == 7 && !Cnd(M, M.ifun)) //jmp failed
                D.bubble = true;
            if (E.icode == 9 || M.icode == 9 || W.icode == 9) //ret
                D.bubble = true;

            if (D.bubble == true) {
                D.SetBubble();
                d.SetBubble();
            }
            //check whether bubble

            if (D.stat != 1) {
                d.Clear();
                d.stat = D.stat;
                OutputDecode();
                return;
            }

            d = D;
            d.srcA = D.rA;
            if (D.icode == 9 || D.icode == 0xb) d.srcA = 4;
            d.srcB = D.rB;
            if (D.icode >= 8) d.srcB = 4;

            switch (D.icode)
            {
                case (2): d.dstE = D.rB; break;
                case (3): d.dstE = D.rB; break;
                case (4): break;
                case (5): d.dstM = D.rA; break;
                case (6): d.dstE = D.rB; break;
                case (7): break;
                case (8): d.dstE = 4; break;
                case (9): d.dstE = 4; break;
                case (0xa): d.dstE = 4; break;
                case (0xb): d.dstE = 4; d.dstM = D.rA; break;
            }
            //		d.srcA = d.dstM = D.rA;
            //		d.srcB = d.dstE = D.rB;

            d.valA = reg[d.srcA];
            d.valB = reg[d.srcB];

            if (D.icode == 7 || D.icode == 8) d.valA = D.valP;
            //jmp or call

            //data forwarding
            if (d.srcA != 0xf)
            {
                if (W.dstE == d.srcA) d.valA = W.valE;
                if (W.dstM == d.srcA) d.valA = W.valM;

                if (M.dstE == d.srcA) d.valA = m.valE;
                if (M.dstM == d.srcA) d.valA = m.valM;

                if (e.dstE == d.srcA) d.valA = e.valE;
            }
            if (d.srcB != 0xf)
            {
                if (W.dstE == d.srcB) d.valB = W.valE;
                if (W.dstM == d.srcB) d.valB = W.valM;
                //from Writeback
                if (M.dstE == d.srcB) d.valB = m.valE;
                if (M.dstM == d.srcB) d.valB = m.valM;
                //from Memory
                if (e.dstE == d.srcB) d.valB = e.valE;
            }
            //if(E.dstM == d.srcA) d.valA = W.valM;
            //if(E.dstM == d.srcB) d.valB = W.valM;

            OutputDecode();
        }
        static void OutputExecute() {
            ClickToChangeValue.ChangeValue(2, "stat", E.stat.ToString());
            ClickToChangeValue.ChangeValue(2, "icode", E.icode.ToString());
            ClickToChangeValue.ChangeValue(2, "ifun", E.ifun.ToString());
            ClickToChangeValue.ChangeValue(2, "valC", E.valC.ToString());
            ClickToChangeValue.ChangeValue(2, "valA", E.valA.ToString());
            ClickToChangeValue.ChangeValue(2, "valB", E.valB.ToString());
            ClickToChangeValue.ChangeValue(2, "dstE", E.dstE.ToString());
            ClickToChangeValue.ChangeValue(2, "dstM", E.dstM.ToString());
            ClickToChangeValue.ChangeValue(2, "srcA", E.srcA.ToString());
            ClickToChangeValue.ChangeValue(2, "srcB", E.srcB.ToString());
            ClickToChangeValue.ChangeCode(2, GetCode(E.pc, E.bubble));
        }
        static void Execute() {
            e = E;
            if (M.icode == 9) E.bubble = true;
            if (M.icode == 7 && !Cnd(M, M.ifun)) E.bubble = true; // jmp failed
            if (D.stall) E.bubble = true;
            if (M.stat != 1 || W.stat != 1) E.bubble = true;
            if (E.bubble == true) {
                E.SetBubble();
                e.SetBubble();
            }

            if (E.stat != 1) {
                e.Clear();
                e.stat = E.stat;
                OutputExecute();
            }

            E.ZF = ZF;
            E.OF = OF;
            E.SF = SF;

            switch (E.icode)
            {
                case (1): break;
                case (2): //rrmovq or cmovq rA,rB
                    e.valE = Alu(E.valA, 0, 0);
                    if (!Cnd(E, E.ifun)) e.dstE = 0xf;
                    break;
                case (3): //irmovq V, rB
                    e.valE = Alu(E.valC, 0, 0);
                    break;
                case (4): //rmmovq rA,D(rB)
                    e.valE = Alu(E.valC, E.valB, 0);
                    break;
                case (5): //mrmovq D(rB),rA
                    e.valE = Alu(E.valC, E.valB, 0);
                    break;
                case (6):
                    e.valE = Alu(E.valA, E.valB, E.ifun);
                    e.set_cc = true;
                    ZF = e.valE == 0;
                    SF = (e.valE >> 63) > 0 ? true : false;
                    OF = ((long)E.valA > 0 && (long)E.valB > 0 && (long)e.valE < 0) || ((long)E.valA < 0 && (long)E.valB < 0 && (long)E.valE >= 0);
                    //(a>0 && b>0 && t<0) || (a<0 && b<0 && t>=0)
                    //set CC
                    break;
                case (7): break;
                case (8): //call
                    e.valE = Alu(0xfffffffffffffff8, E.valB, 0);
                    break;
                case (9): //ret
                    e.valE = Alu(8, E.valB, 0);
                    break;
                case (0xa):
                    e.valE = Alu(0xfffffffffffffff8, E.valB, 0);
                    break;
                case (0xb):
                    e.valE = Alu(8, E.valB, 0);
                    break;
            }
            e.ZF = ZF;
            e.SF = SF;
            e.OF = OF;
            OutputExecute();
        }
        static void OutputMemory()
        {
            ClickToChangeValue.ChangeValue(3, "stat", M.stat.ToString());
            ClickToChangeValue.ChangeValue(3, "icode", M.icode.ToString());
            ClickToChangeValue.ChangeValue(3, "cnd", Cnd(E, E.ifun).ToString());
            ClickToChangeValue.ChangeValue(3, "valE", M.valE.ToString());
            ClickToChangeValue.ChangeValue(3, "valA", M.valA.ToString());
            ClickToChangeValue.ChangeValue(3, "dstE", M.dstE.ToString());
            ClickToChangeValue.ChangeValue(3, "dstM", M.dstM.ToString());
            ClickToChangeValue.ChangeCode(3, GetCode(M.pc, M.bubble));
        }
        static void Memory()
        {
            if (W.icode == 9) M.bubble = true;
            if (M.bubble) {
                M.SetBubble();
                m.SetBubble();
                OutputMemory();
                return;
            }

            if (M.stat != 1 || W.stat != 1) {
                m.Clear();
                m.stat = M.stat;
                OutputMemory();
                return;
            }
            m = M;
            ulong mem_addr = 0;

            bool mem_read = false, mem_write = false;
            switch (M.icode)
            {
                case (4):// rmmovq rA,D(rB)
                    mem_write = true;
                    mem_addr = M.valE;
                    break;
                case (5):// mrmovq D(rB),rA
                    mem_read = true;
                    mem_addr = M.valE;
                    break;
                case (8): // call
                    mem_write = true;
                    mem_addr = M.valE;
                    break;
                case (9): // ret
                    mem_read = true;
                    mem_addr = M.valA;
                    break;
                case (0xa):// push
                    mem_write = true;
                    mem_addr = M.valE;
                    break;
                case (0xb):// pop
                    mem_read = true;
                    mem_addr = M.valA;
                    break;
            }


            if (mem_addr > 800000) {
                m.Clear();
                m.stat = 3;
                OutputMemory();
                return;
            }
            if (mem_write) Mem_sys_write(mem_addr, M.valA);
            if (mem_read) m.valM = Mem_sys_read(mem_addr);
            OutputMemory();
        }

        static void OutputWriteback()
        {
            ClickToChangeValue.ChangeValue(4, "stat", W.stat.ToString());
            ClickToChangeValue.ChangeValue(4, "icode", W.icode.ToString());
            ClickToChangeValue.ChangeValue(4, "valE", W.valE.ToString());
            ClickToChangeValue.ChangeValue(4, "valM", W.valM.ToString());
            ClickToChangeValue.ChangeValue(4, "dstE", W.dstE.ToString());
            ClickToChangeValue.ChangeValue(4, "dstM", W.dstM.ToString());
            ClickToChangeValue.ChangeCode(4, GetCode(W.pc, W.bubble));
        }
        static void Writeback() {
            OutputWriteback();
        }
        public static void Gao() {
            Init();
            //for debug use
            TotalRun();
            OutputReg();
            //debug end
        }
    }
}
public class NextStep : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (conduct.Work.stat == 2)
        {
            UnityEngine.UI.Button errorButton = GameObject.Find("UICanvas/ButtonPanel/ErrorButton").GetComponent<Button>();
            errorButton.onClick.Invoke();
            ErrorHappens.OnClick("运行结束");
        }
        else
        {
            if(conduct.Work.stat == 1) conduct.Work.ClickRun();
            if (conduct.Work.stat != 1 && conduct.Work.stat != 2)
            {
                UnityEngine.UI.Button errorButton = GameObject.Find("UICanvas/ButtonPanel/ErrorButton").GetComponent<Button>();
                errorButton.onClick.Invoke();
                ErrorHappens.OnClick("运行错误");
            }
        }
    }
}
