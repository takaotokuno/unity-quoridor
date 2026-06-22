using NUnit.Framework;
using Quoridor;

namespace Quoridor.Tests
{
    [TestFixture]
    public class SkillButtonSlotMapperTests
    {
        [Test]
        public void GetSkillButtonCount_ExcludesBaseActionSlots()
        {
            Assert.AreEqual(0, SkillButtonSlotMapper.GetSkillButtonCount(0));
            Assert.AreEqual(0, SkillButtonSlotMapper.GetSkillButtonCount(2));
            Assert.AreEqual(3, SkillButtonSlotMapper.GetSkillButtonCount(5));
        }

        [Test]
        public void ToSkillSlotId_MapsButtonIndexAfterBaseActionSlots()
        {
            Assert.AreEqual(3, SkillButtonSlotMapper.ToSkillSlotId(0).Value);
            Assert.AreEqual(5, SkillButtonSlotMapper.ToSkillSlotId(2).Value);
        }

        [Test]
        public void ToButtonIndex_MapsSkillSlotIdToZeroBasedButtonIndex()
        {
            Assert.AreEqual(
                0,
                SkillButtonSlotMapper.ToButtonIndex(new SkillSlotId(3))
            );
            Assert.AreEqual(
                2,
                SkillButtonSlotMapper.ToButtonIndex(new SkillSlotId(5))
            );
        }
    }
}
