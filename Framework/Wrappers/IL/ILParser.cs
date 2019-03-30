using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace NekoClient.Wrappers.IL
{
    internal static class ILParser
    {
        static ILParser()
        {
            FieldInfo[] fields = typeof(OpCodes).GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                OpCode opCode = (OpCode)fields[i].GetValue(null);
                if (opCode.Size == 1)
                {
                    ILParser.OpCodes[(int)opCode.Value] = opCode;
                }
                else
                {
                    ILParser.MultiOpCodes[(int)(opCode.Value & 255)] = opCode;
                }
            }
        }
        
        internal static ILInstruction[] Parse(this MethodInfo method)
        {
            return ILParser.Parse(method.GetMethodBody().GetILAsByteArray(), method.DeclaringType.Assembly.ManifestModule);
        }
        
        internal static ILInstruction[] Parse(this MethodBase methodBase)
        {
            return ILParser.Parse(methodBase.GetMethodBody().GetILAsByteArray(), methodBase.Module);
        }
        
        internal static ILInstruction[] Parse(this MethodBody methodBody, Module manifest)
        {
            return ILParser.Parse(methodBody.GetILAsByteArray(), manifest);
        }
        
        private static ILInstruction[] Parse(byte[] ilCode, Module manifest)
        {
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < ilCode.Length; i++)
            {
                ILInstruction ilinstruction = new ILInstruction((ilCode[i] == 254) ? ILParser.MultiOpCodes[(int)ilCode[i + 1]] : ILParser.OpCodes[(int)ilCode[i]], ilCode, i, manifest);
                arrayList.Add(ilinstruction);
                i += ilinstruction.Length - 1;
            }
            return (ILInstruction[])arrayList.ToArray(typeof(ILInstruction));
        }
        
        private static readonly OpCode[] OpCodes = new OpCode[256];
        
        private static readonly OpCode[] MultiOpCodes = new OpCode[31];
    }
}
