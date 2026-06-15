using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace STAR.DaimlerTrucks
{

    public class CirculationControl : MonoBehaviour
    {
        public List<Transform> particle;
        public RotateTires rotateTires;

        BigCoolerCirculation bigCoolerCirculation;
        BigCirculation bigCirculation;
        SmallCirculation smallCirculation;

        void Awake()
        {
            bigCoolerCirculation = new BigCoolerCirculation();
            bigCirculation = new BigCirculation();
            smallCirculation = new SmallCirculation();
        }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void InitBigCoolerCirculation()
        {
            bigCoolerCirculation.InitCirculation(particle);
            bigCoolerCirculation.SetAnimations(rotateTires);
        }

        public void InitBigCirculation()
        {
            bigCirculation.InitCirculation(particle);
            bigCirculation.SetAnimations(rotateTires);
        }
        public void InitSmallCirculation()
        {
            smallCirculation.InitCirculation(particle);
            smallCirculation.SetAnimations(rotateTires);
        }
    }

    public class CirculationConfig {
        public int[] particleIds;
        public Color color;

        public void InitCirculation(List<Transform> particle) {
            for (int i = 0; i < particle.Count; i++) {
                var value = particleIds.Any(p => p == i);
                if (!value)
                {
                    particle[i].gameObject.SetActive(true);
                    var mainModule = particle[i].GetChild(0).GetComponent<ParticleSystem>().main;
                    //mainModule.startColor = color;
                }
                else
                    particle[i].gameObject.SetActive(false);
            }
        }

        public Color LeapColorTemp(int percentage)
        {
            Color cold = new Color(0, 0, 1); // Blue
            Color hot = new Color(1, 0, 0); // Red

            return Color.Lerp(cold, hot, percentage / 100f);
        }
    }
    
    public class BigCirculation : CirculationConfig
    {
        public BigCirculation() {
            particleIds = new int[] {  };
            //color = LeapColorTemp(90);
        }

        public void SetAnimations(RotateTires rotateTires)
        {
            rotateTires.rotate = false;
            rotateTires.speedZ = 0;
        }
    }

    public class SmallCirculation : CirculationConfig
    {
        public SmallCirculation() {
            particleIds = new int[] {0, 9, 10, 11 };
            //color = LeapColorTemp(20);
        }

        public void SetAnimations(RotateTires rotateTires)
        {
            rotateTires.rotate = false;
            rotateTires.speedZ = 0;
        }
    }

    public class BigCoolerCirculation : CirculationConfig
    {
        public BigCoolerCirculation()
        {
            particleIds = new int[] {  };
            //color = LeapColorTemp(100);
        }

        public void SetAnimations(RotateTires rotateTires)
        {
            rotateTires.rotate = true;
            rotateTires.speedZ = 60;
        }
    }
}
