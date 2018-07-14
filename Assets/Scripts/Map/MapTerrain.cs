using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTerrain
{
    //render of chunks each direction
    public const int RENDER_DISTANCE = 3; //16
    
    public const int WIDTH =  (RENDER_DISTANCE + 2) / 2;  //40  
    public const int DEPTH =  (RENDER_DISTANCE + 2) / 2;  //40
    public const int HEIGHT = (RENDER_DISTANCE + 2) / 2;  //16

    //noise
    public const int NOISE_WIDTH =  ((2 * RENDER_DISTANCE) + 1) * Chunk.WIDTH + 1;
    public const int NOISE_HEIGHT = ((2 * RENDER_DISTANCE) + 1) * Chunk.HEIGHT + 1;
    public const int NOISE_DEPTH =  ((2 * RENDER_DISTANCE) + 1) * Chunk.DEPTH + 1;

    private static List<Chunk> activeChunksList = new List<Chunk>();
    private static Vector3 playerPosition;
    private static double min = double.MaxValue;
    private static double max = double.MinValue;
    private static int normalizeValue = 1;
    public static double isolevel = 1.0;
    public static double[] noise = new double[NOISE_WIDTH * NOISE_HEIGHT * NOISE_DEPTH];

    public bool showGizmos;


    public static void Generate()
    {
        //Init();
        GenerateHeightMap();
        GenerateStartingChunks();
        RenderChunks();
    }

    public static void Init()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    #region OVERRIDES
    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            for (int z = 0; z < 4; z++)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            Gizmos.color = Color.red;
                        else
                            Gizmos.color = Color.white;
                        Gizmos.DrawSphere(new Vector3(x, y, z), 0.1f);
                    }
                }
            }
        }
    }
    #endregion

    #region HELPERS
    private static void GenerateHeightMap()
    {
        #region old way
        //var simplexNoise = new OpenSimplexNoise();

        //for (int z = 0; z < NOISE_DEPTH; z++)
        //{
        //    for (int y = 0; y < NOISE_HEIGHT; y++)
        //    {
        //        for (int x = 0; x < NOISE_WIDTH; x++)
        //        {
        //            #region basic example
        //            //var octaves = 1;
        //            //var deltaFrequency = 0.5;
        //            //var deltaAmplitude = 2.0;
        //            //var deltaScale = 1.0;

        //            //var frequency = 1.0;
        //            //var amplitude = 1.0;
        //            //var scale = 1.0;

        //            //double sample = 0.0;
        //            //for (int i = 0; i < octaves; i++)
        //            //{

        //            //    sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
        //            //                                ((double)y / (double)Chunk.HEIGHT) * frequency,
        //            //                                ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 0.5;

        //            //    frequency *= deltaFrequency;
        //            //    amplitude *= deltaAmplitude;
        //            //    scale *= deltaScale;
        //            //}

        //            //if (sample < min) min = sample;
        //            //if (sample > max) max = sample;

        //            //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
        //            //noise[index] = sample;
        //            #endregion

        //            #region example with overhangs
        //            var octaves = 3;
        //            var deltaFrequency = 0.5;
        //            var deltaAmplitude = 2.0;
        //            var deltaScale = 1.0;

        //            var frequency = 2.0;
        //            var amplitude = 10.0;
        //            var scale = 5.0;

        //            double sample = 0.0;
        //            for (int i = 0; i < octaves; i++)
        //            {

        //                sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
        //                                            ((double)y / (double)Chunk.HEIGHT) * frequency,
        //                                            ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 3f;

        //                frequency *= deltaFrequency;
        //                amplitude *= deltaAmplitude;
        //                scale *= deltaScale;
        //            }

        //            if (sample < min) min = sample;
        //            if (sample > max) max = sample;

        //            var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
        //            noise[index] = sample;
        //            #endregion

        //            #region giant cube example
        //            //double sample = 1.0;

        //            //if (x == 0 || x == NOISE_WIDTH - WIDTH || y == 0 || y == NOISE_HEIGHT - HEIGHT || z == 0 || z == NOISE_DEPTH - DEPTH)
        //            //    sample = -1.0;

        //            //if (sample < min) min = sample;
        //            //if (sample > max) max = sample;

        //            //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
        //            //noise[index] = sample;
        //            #endregion
        //        }
        //    }
        #endregion

        var simplexNoise = new OpenSimplexNoise();

        for (int z = 0; z < NOISE_DEPTH; z++)
        {
            for (int y = 0; y < NOISE_HEIGHT; y++)
            {
                for (int x = 0; x < NOISE_WIDTH; x++)
                {
                    #region basic example
                    //var octaves = 1;
                    //var deltaFrequency = 0.5;
                    //var deltaAmplitude = 2.0;
                    //var deltaScale = 1.0;

                    //var frequency = 1.0;
                    //var amplitude = 1.0;
                    //var scale = 1.0;

                    //double sample = 0.0;
                    //for (int i = 0; i < octaves; i++)
                    //{

                    //    sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
                    //                                ((double)y / (double)Chunk.HEIGHT) * frequency,
                    //                                ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 0.5;

                    //    frequency *= deltaFrequency;
                    //    amplitude *= deltaAmplitude;
                    //    scale *= deltaScale;
                    //}

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = sample;
                    #endregion

                    #region example with overhangs
                    //var octaves = 3;
                    //var deltaFrequency = 0.5;
                    //var deltaAmplitude = 2.0;
                    //var deltaScale = 1.0;

                    //var frequency = 2.0;
                    //var amplitude = 10.0;
                    //var scale = 5.0;

                    //double sample = 0.0;
                    //for (int i = 0; i < octaves; i++)
                    //{

                    //    sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
                    //                                ((double)y / (double)Chunk.HEIGHT) * frequency,
                    //                                ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 3f;

                    //    frequency *= deltaFrequency;
                    //    amplitude *= deltaAmplitude;
                    //    scale *= deltaScale;
                    //}

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = sample;
                    #endregion

                    #region giant cube example
                    double sample = 1.0;

                    if (x == 0 || x == NOISE_WIDTH - 1 || y == 0 || y == NOISE_HEIGHT - 1 || z == 0 || z == NOISE_DEPTH - 1)
                        sample = -1.0;

                    if (sample < min) min = sample;
                    if (sample > max) max = sample;

                    var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    noise[index] = sample;
                    #endregion

                    #region flat
                    //double sample = -1.0;

                    ////if (y <= ((RENDER_DISTANCE + 2) / 2) * Chunk.HEIGHT)
                    //if (y == 0)
                    //{
                    //    sample = 1.0;
                    //}

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = sample;
                    #endregion  
                }
            }
        }

        double average = (min + max) / 2;
        double range = (max - min) / 2;

        for (int z = 0; z < NOISE_DEPTH; z++)
        {
            for (int y = 0; y < NOISE_HEIGHT; y++)
            {
                for (int x = 0; x < NOISE_WIDTH; x++)
                {
                    var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    noise[index] = (noise[index] - average) / range * normalizeValue;
                }
            }
        }
    }
    private static void GenerateStartingChunks()
    {
        #region old way
        //for (int z = -DEPTH, noiseZ = 0; z < DEPTH + 1; z++, noiseZ++)
        //{
        //    for (int y = -HEIGHT, noiseY = 0; y < HEIGHT + 1; y++, noiseY++)
        //    {
        //        for (int x = -WIDTH, noiseX = 0; x < WIDTH + 1; x++, noiseX++)
        //        {
        //            var chunkX = x * Chunk.WIDTH;
        //            var chunkY = y * Chunk.HEIGHT;
        //            var chunkZ = z * Chunk.DEPTH;

        //            var noiseVector = new Vector3(noiseX * Chunk.WIDTH, noiseY * Chunk.HEIGHT, noiseZ * Chunk.DEPTH);
        //            var go = new GameObject(chunkX + "," + chunkY + "," + chunkZ);
        //            go.transform.position = new Vector3(chunkX, chunkY, chunkZ);

        //            var chunk = new Chunk(go, noiseVector);
        //            activeChunksList.Add(chunk);
        //            chunk.GenerateTerrain();
        //        }
        //    }
        //}
        #endregion

        #region sprial 
        var x = (2 * RENDER_DISTANCE) + 1;
        var y = (2 * RENDER_DISTANCE) + 1;
        var z = (2 * RENDER_DISTANCE) + 1;
        Spiral(x, y, z);
        #endregion
    }
    private static void Spiral(int X, int Y, int Z)
    {
        bool flipped = false;
        int y = 0;
        while (y > -Y + RENDER_DISTANCE)
        {
            int x, z, dx, dz;
            x = z = dx = 0;
            dz = -1;
            int maxI = X * Z;
            for (int i = 0; i < maxI; i++)
            {
                var chunkX = x * Chunk.WIDTH;
                var chunkY = y * Chunk.HEIGHT;
                var chunkZ = z * Chunk.DEPTH;

                var noiseX = (x + RENDER_DISTANCE) * Chunk.WIDTH;
                var noiseY = (y + RENDER_DISTANCE) * Chunk.HEIGHT;
                var noiseZ = (z + RENDER_DISTANCE) * Chunk.DEPTH;

                var noiseVector = new Vector3(noiseX, noiseY, noiseZ);
                var go = new GameObject(chunkX + "," + chunkY + "," + chunkZ);
                go.transform.position = new Vector3(chunkX, chunkY, chunkZ);

                var chunk = new Chunk(go, noiseVector);
                activeChunksList.Add(chunk);
                chunk.GenerateTerrain();

                if ((x == z) ||
                    ((x < 0) && (x == -z)) ||
                    ((x > 0) && (x == 1 - z)))
                {
                    var t = dx;
                    dx = -dz;
                    dz = t;

                }

                //Mutate x,y,z at the end
                x += dx;
                z += dz;
            }
            if (y == Y/2)
            {
                y = 0;
                flipped = true;
            }
            if (!flipped)
                y++;
            else
                y--;
        }

    }
    
    private static void RenderChunks()
    {
        foreach (var chunk in activeChunksList)
        {
            chunk.Render();
        }
    }
    #endregion


}