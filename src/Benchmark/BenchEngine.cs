using System;

namespace Benchmark
{
    public class BenchEngine
    {
        private readonly IObjectToObjectMapper _mapper;
        private readonly string _mode;

        public BenchEngine(IObjectToObjectMapper mapper, string mode)
        {
            _mapper = mapper;
            _mode = mode;
        }

        public void Start()
        {
            var timer = new HiPerfTimer();

            _mapper.Initialize();

            _mapper.Map();

            timer.Start();

            for (var i = 0; i < 1000000; i++)
            {
                _mapper.Map();
            }

            timer.Stop();

            Console.WriteLine($"{_mapper.Name}: - {_mode} - Mapping time: \t{timer.Duration}s");
        }
    }
}