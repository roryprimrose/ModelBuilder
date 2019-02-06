namespace ModelBuilder.UnitTests
{
    using System;

    public class PopulateOrderItem
    {
        private Person _w;
        private string _x;
        private int _y = int.MinValue;

        public Person W
        {
            get { return _w; }
            set
            {
                if (_x == null)
                {
                    throw new InvalidOperationException(
                        "Execution order was not run as expected because the string was not assigned before the class");
                }

                _w = value;
            }
        }

        public string X
        {
            get { return _x; }
            set
            {
                if (_y == int.MinValue)
                {
                    throw new InvalidOperationException(
                        "Execution order was not run as expected because the int was not assigned before the string");
                }

                _x = value;
            }
        }

        public int Y
        {
            get { return _y; }
            set
            {
                if (Z == (SimpleEnum) int.MinValue)
                {
                    throw new InvalidOperationException(
                        "Execution order was not run as expected because the enum was not assigned before the int");
                }

                _y = value;
            }
        }

        public SimpleEnum Z { get; set; } = (SimpleEnum) int.MinValue;
    }
}