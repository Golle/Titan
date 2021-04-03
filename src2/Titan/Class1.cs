using System;
using Titan.Core.Memory;

namespace Titan
{
    public struct TestStruct
    {

        public float A;
        public long B;
        public bool C;

        public override string ToString() => $"A:{A} B:{B} C:{C}";
    }

    public class Class1
    {


        public void Run()
        {


            {
                var block = MemoryUtils.AllocateBlock(1000);

                unsafe
                {
                    var ptr = (int*) block;

                    *ptr = 1000;

                    Console.WriteLine(ptr[0]);
                }
                

                block.Free();
            }

            {
                var block = MemoryUtils.AllocateBlock<TestStruct>(10, true);
                unsafe
                {
                    var ptr = block.AsPointer() +3;
                    *ptr = new TestStruct
                    {
                        A = 10,
                        B = 200000000,
                        C = true
                    };
                }

                Console.WriteLine(block[2]);
                block.Free();
            }

            


        }
    }
}
