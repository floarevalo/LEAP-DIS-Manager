// Copyright 2008-2011. This work is licensed under the BSD license, available at
// http://www.movesinstitute.org/licenses
//
// Orignal authors: DMcG, Jason Nelson
// Modified for use with C#:
// - Peter Smith (Naval Air Warfare Center - Training Systems Division)
// - Zvonko Bostjancic (Blubit d.o.o. - zvonko.bostjancic@blubit.si)

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenDis.Enumerations.Entity.Information.Minefield
{
    /// <summary>
    /// Enumeration values for Fusing (entity.mine.fusing, Fusing,
    /// section 10.3.3)
    /// The enumeration values are generated from the SISO DIS XML EBV document (R35), which was
    /// obtained from http://discussions.sisostds.org/default.asp?action=10&amp;fd=31
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Due to SISO standardized naming.")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Due to SISO standardized naming.")]
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
    [Serializable]
    public struct Fusing : IHashable<Fusing>
    {
        /// <summary>
        /// Identifies the type of the primary fuse
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
        [Description("Identifies the type of the primary fuse")]
        public enum PrimaryValue : uint
        {
            /// <summary>
            /// No Fuse
            /// </summary>
            NoFuse = 0,

            /// <summary>
            /// Other
            /// </summary>
            Other = 1,

            /// <summary>
            /// Pressure
            /// </summary>
            Pressure = 2,

            /// <summary>
            /// Magnetic
            /// </summary>
            Magnetic = 3,

            /// <summary>
            /// Tilt Rod
            /// </summary>
            TiltRod = 4,

            /// <summary>
            /// Command
            /// </summary>
            Command = 5,

            /// <summary>
            /// Trip Wire
            /// </summary>
            TripWire = 6
        }

        /// <summary>
        /// Identifies the type of the secondary fuse
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
        [Description("Identifies the type of the secondary fuse")]
        public enum SecondaryValue : uint
        {
            /// <summary>
            /// No Fuse
            /// </summary>
            NoFuse = 0,

            /// <summary>
            /// Other
            /// </summary>
            Other = 1,

            /// <summary>
            /// Pressure
            /// </summary>
            Pressure = 2,

            /// <summary>
            /// Magnetic
            /// </summary>
            Magnetic = 3,

            /// <summary>
            /// Tilt Rod
            /// </summary>
            TiltRod = 4,

            /// <summary>
            /// Command
            /// </summary>
            Command = 5,

            /// <summary>
            /// Trip Wire
            /// </summary>
            TripWire = 6
        }

        /// <summary>
        /// Describes the anti-handling device status of the mine
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Due to SISO standardized naming.")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Due to SISO standardized naming.")]
        [Description("Describes the anti-handling device status of the mine")]
        public enum AHDValue : uint
        {
            /// <summary>
            /// No anti-handling device
            /// </summary>
            NoAntiHandlingDevice = 0,

            /// <summary>
            /// Anti-handling device
            /// </summary>
            AntiHandlingDevice = 1
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        ///    <c>true</c> if operands are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Fusing left, Fusing right) => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        ///    <c>true</c> if operands are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Fusing left, Fusing right)
            => ReferenceEquals(left, right) || left.Equals(right);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Fusing"/> to <see cref="ushort"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Fusing"/> scheme instance.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator ushort(Fusing obj) => obj.ToUInt16();

        /// <summary>
        /// Performs an explicit conversion from <see cref="ushort"/> to <see cref="Fusing"/>.
        /// </summary>
        /// <param name="value">The ushort value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Fusing(ushort value) => FromUInt16(value);

        /// <summary>
        /// Creates the <see cref="Fusing"/> instance from the byte array.
        /// </summary>
        /// <param name="array">The array which holds the values for the <see cref="Fusing"/>.</param>
        /// <param name="index">The starting position within value.</param>
        /// <returns>The <see cref="Fusing"/> instance, represented by a byte array.</returns>
        /// <exception cref="ArgumentNullException">if the <c>array</c> is null.</exception>
        /// <exception cref="IndexOutOfRangeException">if the <c>index</c> is lower than 0 or greater or equal than number
        /// of elements in array.</exception>
        public static Fusing FromByteArray(byte[] array, int index) => array == null
                ? throw new ArgumentNullException(nameof(array))
                : index < 0 ||
                index > array.Length - 1 ||
                index + 2 > array.Length - 1
                ? throw new IndexOutOfRangeException()
                : FromUInt16(BitConverter.ToUInt16(array, index));

        /// <summary>
        /// Creates the <see cref="Fusing"/> instance from the ushort value.
        /// </summary>
        /// <param name="value">The ushort value which represents the <see cref="Fusing"/> instance.</param>
        /// <returns>The <see cref="Fusing"/> instance, represented by the ushort value.</returns>
        public static Fusing FromUInt16(ushort value)
        {
            var ps = new Fusing();

            const uint mask0 = 0x007f;
            const byte shift0 = 0;
            uint newValue0 = (value & mask0) >> shift0;
            ps.Primary = (PrimaryValue)newValue0;

            const uint mask1 = 0x3f80;
            const byte shift1 = 7;
            uint newValue1 = (value & mask1) >> shift1;
            ps.Secondary = (SecondaryValue)newValue1;

            const uint mask2 = 0x0010;
            const byte shift2 = 4;
            uint newValue2 = (value & mask2) >> shift2;
            ps.AHD = (AHDValue)newValue2;

            return ps;
        }

        /// <summary>
        /// Gets or sets the primary.
        /// </summary>
        /// <value>The primary.</value>
        public PrimaryValue Primary { get; set; }

        /// <summary>
        /// Gets or sets the secondary.
        /// </summary>
        /// <value>The secondary.</value>
        public SecondaryValue Secondary { get; set; }

        /// <summary>
        /// Gets or sets the ahd.
        /// </summary>
        /// <value>The ahd.</value>
        public AHDValue AHD { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Fusing other && Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="Fusing"/> instance is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Fusing"/> instance to compare with this instance.</param>
        /// <returns>
        ///    <c>true</c> if the specified <see cref="Fusing"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Fusing other) =>
            // If parameter is null return false (cast to object to prevent recursive loop!)
            Primary == other.Primary &&
                Secondary == other.Secondary &&
                AHD == other.AHD;

        /// <summary>
        /// Converts the instance of <see cref="Fusing"/> to the byte array.
        /// </summary>
        /// <returns>The byte array representing the current <see cref="Fusing"/> instance.</returns>
        public byte[] ToByteArray() => BitConverter.GetBytes(ToUInt16());

        /// <summary>
        /// Converts the instance of <see cref="Fusing"/> to the ushort value.
        /// </summary>
        /// <returns>The ushort value representing the current <see cref="Fusing"/> instance.</returns>
        public ushort ToUInt16()
        {
            ushort val = 0;

            val |= (ushort)((uint)Primary << 0);
            val |= (ushort)((uint)Secondary << 7);
            val |= (ushort)((uint)AHD << 4);

            return val;
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            int hash = 17;

            // Overflow is fine, just wrap
            unchecked
            {
                hash = (hash * 29) + Primary.GetHashCode();
                hash = (hash * 29) + Secondary.GetHashCode();
                hash = (hash * 29) + AHD.GetHashCode();
            }

            return hash;
        }
    }
}
