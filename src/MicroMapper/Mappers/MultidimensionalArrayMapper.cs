namespace MicroMapper.Mappers
{
    using System;
    using Internal;

    public class MultidimensionalArrayMapper : EnumerableMapperBase<Array>
    {
        //TODO: this one is entirely new

        public override bool IsMatch(ResolutionContext context)
        {
            return context.DestinationType.IsArray
                && context.DestinationType.GetArrayRank() > 1
                && context.SourceType.IsEnumerableType();
        }

        protected override void ClearEnumerable(Array enumerable)
        {
            // no op
        }

        private MultidimensionalArrayFiller _filler;

        protected override void SetElementValue(Array destination, object mappedValue, int index)
        {
            _filler.NewValue(mappedValue);
        }

        protected override Array CreateDestinationObjectBase(Type destElementType, int sourceLength)
        {
            throw new NotImplementedException();
        }

        protected override object GetOrCreateDestinationObject(ResolutionContext context,
            Type destElementType, int sourceLength)
        {
            var sourceArray = context.SourceValue as Array;
            if(sourceArray == null)
            {
                return ObjectCreator.CreateArray(destElementType, sourceLength);
            }
            var destinationArray = ObjectCreator.CreateArray(destElementType, sourceArray);
            _filler = new MultidimensionalArrayFiller(destinationArray);
            return destinationArray;
        }
    }

    public class MultidimensionalArrayFiller
    {
        private readonly int[] _indeces;
        private readonly Array _destination;

        public MultidimensionalArrayFiller(Array destination)
        {
            _indeces = new int[destination.Rank];
            _destination = destination;
        }

        public void NewValue(object value)
        {
            var dimension = _destination.Rank - 1;
            var changedDimension = false;
            while(_indeces[dimension] == _destination.GetLength(dimension))
            {
                _indeces[dimension] = 0;
                dimension--;
                if(dimension < 0)
                {
                    throw new InvalidOperationException("Not enough room in _destination array " + _destination);
                }
                _indeces[dimension]++;
                changedDimension = true;
            }
            _destination.SetValue(value, _indeces);
            if(changedDimension)
            {
                _indeces[dimension+1]++;
            }
            else
            {
                _indeces[dimension]++;
            }
        }
    }
}