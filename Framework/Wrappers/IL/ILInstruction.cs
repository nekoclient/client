using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NekoClient.Wrappers.IL
{
    internal struct ILInstruction
    {
        internal ILInstruction(OpCode opCode, byte[] ilCode, int index, Module manifest)
        {
            this.OpCode = opCode;
            this.HasArgument = (opCode.OperandType != OperandType.InlineNone);
            this.HasSingleByteArgument = (OpCodes.TakesSingleByteArgument(opCode) && this.OpCode != OpCodes.Ldc_R4);
            this.Length = opCode.Size + (this.HasArgument ? (this.HasSingleByteArgument ? 1 : 4) : 0);
            if (this.HasArgument)
            {
                if (this.HasSingleByteArgument)
                {
                    this.Argument = ilCode[index + opCode.Size];
                }
                else if (this.OpCode == OpCodes.Ldc_R4)
                {
                    this.Argument = BitConverter.ToSingle(ilCode, index + opCode.Size);
                }
                else
                {
                    this.Argument = BitConverter.ToInt32(ilCode, index + opCode.Size);
                }
                if (this.OpCode == OpCodes.Ldstr)
                {
                    this.Argument = manifest.ResolveString((int)this.Argument);
                    return;
                }
                if (this.OpCode == OpCodes.Call || this.OpCode == OpCodes.Callvirt || this.OpCode == OpCodes.Ldftn || this.OpCode == OpCodes.Newobj)
                {
                    this.Argument = manifest.ResolveMethod((int)this.Argument);
                    return;
                }
                if (this.OpCode == OpCodes.Box || this.OpCode == OpCodes.Newarr)
                {
                    this.Argument = manifest.ResolveType((int)this.Argument);
                    return;
                }
                if (this.OpCode == OpCodes.Ldfld || this.OpCode == OpCodes.Stfld || this.OpCode == OpCodes.Ldflda || this.OpCode == OpCodes.Ldsfld || this.OpCode == OpCodes.Stsfld || this.OpCode == OpCodes.Ldsflda)
                {
                    this.Argument = manifest.ResolveField((int)this.Argument);
                    return;
                }
            }
            else
            {
                this.Argument = null;
            }
        }
        
        internal T GetArgument<T>()
        {
            return (T)((object)this.Argument);
        }
        
        public override string ToString()
        {
            string arg = string.Empty;
            if (this.HasArgument)
            {
                if (this.Argument is int || this.Argument is byte)
                {
                    arg = string.Format(" 0x{0:X}", this.Argument);
                }
                else if (this.Argument is string)
                {
                    arg = " \"" + this.Argument.ToString() + "\"";
                }
                else
                {
                    arg = " " + this.Argument.ToString();
                }
            }
            return string.Format("{0}{1}", this.OpCode.Name, arg);
        }

        internal readonly OpCode OpCode;

        internal readonly object Argument;

        internal readonly bool HasArgument;

        internal readonly bool HasSingleByteArgument;

        internal readonly int Length;
    }
}
