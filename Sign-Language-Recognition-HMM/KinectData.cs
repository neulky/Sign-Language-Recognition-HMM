using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Sign_Language_Recognition_HMM
{
    class KinectData
    {
        public double[][][] kinectdata_seq;
        private KinectSensor sensor;
        public void GetKinectData()
        {
            sensor = KinectSensor.KinectSensors[0];
            sensor.SkeletonStream.Enable();
            sensor.Start();

            List<List<double[]>> kinectdata_list= new List<List<double[]>>();
            List<double[]> kinectdata_left = new List<double[]>();
            List<double[]> kinectdata_right = new List<double[]>();

            int leftStartFlag = 0;   //开始标志  当开始记录帧时，startFlag置为1；当再次出现手部坐标为（0，0）且startFlag=1时，结束记录
            int rightStartFlag = 0;

            while(true)
            {
                SkeletonFrame skframe = sensor.SkeletonStream.OpenNextFrame(int.MaxValue);
                if( skframe != null)
                {
                    Skeleton[] FrameSkeletons = new Skeleton[skframe.SkeletonArrayLength];
                    skframe.CopySkeletonDataTo(FrameSkeletons);

                    Skeleton user = (from sk in FrameSkeletons
                                     where sk.TrackingState == SkeletonTrackingState.Tracked
                                     select sk).FirstOrDefault();
                    if (user != null)
                    {
                        Joint handLeft = user.Joints[JointType.HandLeft];
                        Joint handRight = user.Joints[JointType.HandRight];
                        
                        DepthImagePoint handLeftDepthImage = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(handLeft.Position, sensor.DepthStream.Format);
                        DepthImagePoint handRightDepthImage = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(handRight.Position, sensor.DepthStream.Format);
                        
                        Joint hipCenter = user.Joints[JointType.HipCenter];
                        DepthImagePoint hipCenterDepthImage = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(hipCenter.Position, sensor.DepthStream.Format);

                        if(handLeftDepthImage.Y > hipCenterDepthImage.Y)       //假设当手部低于胯部是 手的坐标为（0，0）
                        {
                            handLeftDepthImage.X = 0;
                            handLeftDepthImage.Y = 0;
                        }
                        if(handRightDepthImage.Y > hipCenterDepthImage.Y)
                        {
                            handRightDepthImage.X = 0;
                            handRightDepthImage.Y = 0;
                        }

                        //Console.WriteLine("({0},{1}) ({2},{3})", handLeftDepthImage.X, handLeftDepthImage.Y, handRightDepthImage.X, handRightDepthImage.Y);

                        if (handRightDepthImage.X == 0 && handRightDepthImage.Y == 0 &&
                                handLeftDepthImage.X == 0 && handLeftDepthImage.Y == 0 && (rightStartFlag == 1 || leftStartFlag == 1))    //两只手同时都在下面，且曾经有一只手上去过
                            break;

                        if (handRightDepthImage.X == 0 && handRightDepthImage.Y == 0 && rightStartFlag == 0)    
                        {
                            
                        }
                        else
                        {
                            rightStartFlag = 1;
                            if (handRightDepthImage.X != 0 && handRightDepthImage.Y != 0)
                            {
                                double[] point  = new double[2];
                                point[0] =  handRightDepthImage.X;
                                point[1] = handRightDepthImage.Y;
                                kinectdata_right.Add(point);
                            }
                        }
                        if (handLeftDepthImage.X == 0 && handLeftDepthImage.Y == 0 && leftStartFlag == 0)    
                        {

                        }
                        else
                        {
                            leftStartFlag = 1;
                            if (handLeftDepthImage.X != 0 && handLeftDepthImage.Y != 0)
                            {
                                double[] point = new double[2];
                                point[0] = handLeftDepthImage.X;
                                point[1] = handLeftDepthImage.Y;
                                kinectdata_left.Add(point);
                            }
                        }
                    }
                }
            }

            kinectdata_list.Add(kinectdata_left);       //先左
            kinectdata_list.Add(kinectdata_right);      //后右

            double[][][] kinectdata_temp = new double[kinectdata_list.Count][][];
            for (int i = 0; i < kinectdata_list.Count;i++)
            {
                kinectdata_temp[i] = new double[kinectdata_list[i].Count][];
                for(int j = 0;j < kinectdata_list[i].Count;j++)
                {
                    kinectdata_temp[i][j] = new double[kinectdata_list[i][j].Length];
                    for(int k = 0;k < kinectdata_list[i][j].Length;k++)
                    {
                        kinectdata_temp[i][j][k] = kinectdata_list[i][j][k];
                    }
                }
            }

            kinectdata_seq = kinectdata_temp;              //将采集到的左右手数据赋给 共有属性kinectdata

            sensor.Stop();

        }

    }
}