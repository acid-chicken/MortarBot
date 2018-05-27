using System;
using Xunit;
using static MortarBot.MortarMath;

namespace MortarBot.Tests
{
    public class MortarMathTest
    {
        [Fact]
        public void ItemTest()
        {
            Assert.Equal(1m, Calculate("1"));
            Assert.Equal(2m, Calculate("1+1"));
            Assert.Equal(0m, Calculate("1-1"));
        }

        [Fact]
        public void FactorTest()
        {
            Assert.Equal(1m, Calculate("1*1"));
            Assert.Equal(-1m, Calculate("1*-1"));
            Assert.Equal(-1m, Calculate("-1*1"));
            Assert.Equal(1m, Calculate("-1*-1"));
            Assert.Equal(1m, Calculate("1/1"));
            Assert.Equal(-1m, Calculate("1/-1"));
            Assert.Equal(-1m, Calculate("-1/1"));
            Assert.Equal(1m, Calculate("-1/-1"));
            Assert.Equal(1m, Calculate("(1)(1)"));
            Assert.Equal(-1m, Calculate("(1)(-1)"));
            Assert.Equal(-1m, Calculate("(-1)(1)"));
            Assert.Equal(1m, Calculate("(-1)(-1)"));
            Assert.Equal(1m, Calculate("(1)1"));
            Assert.Equal(-1m, Calculate("(-1)1"));
            Assert.Equal(1m, Calculate("1(1)"));
            Assert.Equal(-1m, Calculate("1(-1)"));
            Assert.Equal(-1m, Calculate("-1(1)"));
            Assert.Equal(1m, Calculate("-1(-1)"));
        }

        [Fact]
        public void ConstantTest()
        {
            Assert.Equal(2000m, Calculate("maxcpu"));
            Assert.Equal(2000m, Calculate("maxcpu+maxcpu-maxcpu*maxcpu/maxcpu"));
            Assert.Equal(16000000000000m, Calculate("maxcpu(maxcpu)(maxcpu)maxcpu"));
        }

        [Fact]
        public void FunctionTest()
        {
            Assert.InRange(Calculate("d6"), 1m, 6m);
            Assert.InRange(Calculate("2d6"), 2m, 12m);
        }
    }
}
