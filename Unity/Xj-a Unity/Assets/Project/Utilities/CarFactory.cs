using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
//using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Project.Utilities
{
    public class CarFactory : Singleton<CarFactory>
    {
        // Start is called before the first frame update
        internal static CarFactory instance;
        public GameObject carPrefab;
        public AbstractMap abstractMap;

        void Start()
        {

        }

        public void CreateCar(float lat, float lon)
        {
            Debug.Log(String.Format("create car, {0}, {1}", lat, lon));
            Vector2d position = new Vector2d(lat, lon);
            var car = Instantiate(carPrefab);
            car.transform.position = abstractMap.GeoToWorldPosition(position, true);

        }

        

        // Update is called once per frame
        void Update()
        {

        }
    }
}