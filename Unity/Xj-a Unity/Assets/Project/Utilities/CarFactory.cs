using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Project.Utilities
{
    public class CarFactory : Singleton<CarFactory>
    {
        // Start is called before the first frame update
        internal static CarFactory instance;
        private Car[] cars;

        void Start()
        {
            instance = this.Instance;
        }

        private void Awake()
        {
            Assert.IsNotNull(cars);
        }

        public void CreateCar(float lat, float lon)
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}