using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MDPExcersise
{
    class Position{
        public int x {get; set;}
        public int y {get; set;}

        public Position(int x, int y){
            this.x = x;
            this.y = y;
        }
        public Position(){
            this.x = 0;
            this.y = 0;
        }
    }
    class Node{
        public float value;
        public string direction;
        public float reward;
        public int x {get; set;}
        public int y {get; set;}

        public Node(int x, int y){
            this.x = x;
            this.y = y;
            this.value = -0.04f;
            this.reward = -0.04f;
            this.direction = "R";
        }
        public Node(int x, int y, float reward){
            this.x = x;
            this.y = y;
            this.value = -0.04f;
            this.reward = reward;
            this.direction = "R";
        }

        public override string ToString(){
            return "Cell("+this.x+","+this.y+")"+" "+value;
        }
    }
    
    class MDPGrid{
        public Node[,] grid;
        public Node[,] prevGrid;
        public float alpha = 0.9f;
        public string prevState="";
        public string State="";

        public List<List<object>> history = new List<List<object>>();
        public List<List<object>> directionHistory = new List<List<object>>();

        public IDictionary<int, string> dict = new Dictionary<int, string>();
        public IDictionary<string, string> flechas = new Dictionary<string, string>();

        
        public MDPGrid(int width, int height){
            this.grid = new Node[width,height];
            for(int i = 0; i<grid.GetLength(0);i++){
                for(int j = 0; j<grid.GetLength(1);j++){
                    this.grid[i,j] = new Node(i,j);
                }
            }
            
            this.dict.Add(0,"R");
            this.dict.Add(1,"UR");
            this.dict.Add(2,"U");
            this.dict.Add(3,"UL");
            this.dict.Add(4,"L");
            this.dict.Add(5,"DL");
            this.dict.Add(6,"D");
            this.dict.Add(7,"DR");
            this.dict.Add(8,"S");

            this.flechas.Add("R","→");
            this.flechas.Add("UR","↗");
            this.flechas.Add("U","↑");
            this.flechas.Add("UL","↖");
            this.flechas.Add("L","←");
            this.flechas.Add("DL","↙");
            this.flechas.Add("D","↓");
            this.flechas.Add("DR","↘");
            this.flechas.Add("S","S");
        }

        public MDPGrid(int width, int height, string filePath){
            this.grid = new Node[width,height];
            string inputPath=filePath;
            var reader = new StreamReader(File.OpenRead(inputPath));
            List<List<float>> listA = new List<List<float>>();
            float[,] rewardsInput = new float[width,height]; 
            int rowC = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                List<float> rowValues = new List<float>();
                for (int column = 0; column<values.Length;column++)
                {
                    rewardsInput[rowC,column]=float.Parse(values[column]);
                }
                rowC++;
            }
            
            this.dict.Add(0,"R");
            this.dict.Add(1,"UR");
            this.dict.Add(2,"U");
            this.dict.Add(3,"UL");
            this.dict.Add(4,"L");
            this.dict.Add(5,"DL");
            this.dict.Add(6,"D");
            this.dict.Add(7,"DR");
            this.dict.Add(8,"S");

            this.flechas.Add("R","→");
            this.flechas.Add("UR","↗");
            this.flechas.Add("U","↑");
            this.flechas.Add("UL","↖");
            this.flechas.Add("L","←");
            this.flechas.Add("DL","↙");
            this.flechas.Add("D","↓");
            this.flechas.Add("DR","↘");
            this.flechas.Add("S","S");
            
            for(int row = 0; row<grid.GetLength(0);row++){
                for(int column = 0; column<grid.GetLength(1);column++){
                    this.grid[row,column] = new Node(row,column,rewardsInput[row,column]);
                }
            }
        }

        public void setWalls(Position[] wallPositions){
            for(int i = 0 ; i<wallPositions.GetLength(0);i++){
                this.grid[wallPositions[i].x,wallPositions[i].y].value = -10;
                this.grid[wallPositions[i].x,wallPositions[i].y].reward = -10;
            }
        }

        public void setGoals(Position[] goalsPositions, int[] goalValues){
            for(int i = 0 ; i<goalsPositions.GetLength(0);i++){
                this.grid[goalsPositions[i].x,goalsPositions[i].y].value = goalValues[i];
                this.grid[goalsPositions[i].x,goalsPositions[i].y].reward = goalValues[i];
            }
        }

        public float checkBounds(int x, int y){
            if(x<0 || y<0){
                return -10;
            }
            if(x >= this.grid.GetLength(0) || y >= this.grid.GetLength(1)){
                return 0;
            }
            return this.grid[x,y].value;
        }
        public float MyMax(params float[] values)
        {
            return values.Max();
        }
        public string getDirection(params float[] values){
            float maxValue = values.Max();
            int maxIndex = values.ToList().IndexOf(maxValue);
            return this.dict[maxIndex];
        }
        public void newValue(int i, int j){//example 0,2
            float cellValue = this.grid[i,j].reward + this.alpha*MyMax(
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.9f+ checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.9f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.9f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.9f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.9f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.9f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.9f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.9f,2)
            );
            String direction = getDirection(
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.9f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.9f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.9f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.9f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.9f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.9f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.9f + checkBounds(i+1,j+1)*.01f,2),
                MathF.Round(checkBounds(i,j)*.01f + checkBounds(i,j+1)*.01f + checkBounds(i-1,j+1)*.01f + checkBounds(i-1,j)*.01f + checkBounds(i-1,j-1)*.01f + checkBounds(i,j-1)*.01f + checkBounds(i+1,j-1)*.01f + checkBounds(i+1,j)*.01f + checkBounds(i+1,j+1)*.9f,2)
            );
            this.grid[i,j].direction = direction;
            this.grid[i,j].value = cellValue;
        }
        public void saveState(int ic, int jc){
            List<object> newentry = new List<object>();
            List<object> newentryDirection = new List<object>();
            this.prevState = this.State;
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    newentry.Add(this.grid[i,j].value);
                    newentryDirection.Add(this.grid[i,j].direction);
                }
                this.history.Add(newentry);
                this.directionHistory.Add(newentryDirection);
                newentry = new List<object>();
                newentryDirection = new List<object>();
            }
            newentry = new List<object>(){"value",this.grid[ic,jc],"-","-","-","-","-","-"};
            this.history.Add(newentry);
            this.directionHistory.Add(newentry);
            string nextState="";
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    nextState+=this.grid[i,j].direction;
                }   
            }
            this.State = nextState;


        }
        public void copyTable(){
            this.prevGrid = new Node[this.grid.GetLength(0),this.grid.GetLength(1)];
            for (int row = 0; row < this.grid.GetLength(0); row++)
            {
                for (int column = 0; column < this.grid.GetLength(1); column++)
                {
                    this.prevGrid[row,column] = new Node(row,column);
                    this.prevGrid[row,column].direction = this.grid[row,column].direction;
                    this.prevGrid[row,column].reward = this.grid[row,column].reward;
                    this.prevGrid[row,column].value = this.grid[row, column].value;
                }
            }
        }
        public void evalGrid(){
            this.copyTable();
            for(int i = 0; i<this.grid.GetLength(0);i++){
                Console.WriteLine("row: " + i);
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    newValue(i,j);
                    saveState(i,j);
                }
            }
        }
        public string drawGrid(){
            string grid="";
            grid += "-------------Values-------------\n";
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    grid+= string.Format("{0:N2}",this.grid[i,j].value) + "\t|";
                }
                grid += "\n";
                grid += "----------------------------------------------\n";
            }
            
            return grid;
        }

        public string drawGridDirection(){
            string grid="";
            grid += "-------------Policy-------------\n";
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    if(this.grid[i,j].reward == -10){
                        grid+= "   W\t|";
                    }else{
                        if(this.grid[i,j].reward > 0){
                            grid+= "   G\t|";
                        }else{
                            grid+= string.Format("{0:N2}",this.grid[i,j].direction) + "\t|";
                        }
                    }
                    
                }
                grid += "\n";
                grid += "----------------------------------------------\n";
            }
            
            return grid;
        }

        public string createPolicy() {
            string grid = "";

            for (int i = 0; i < this.grid.GetLength(0); i++)
            {
                for (int j = 0; j < this.grid.GetLength(1); j++)
                {
                    grid += string.Format("{0:N2}", this.grid[i, j].direction) + ",";
                }
                grid = grid.Remove(grid.Length -1);
                grid += "\n";
            }
            return grid;
        }

        public override string ToString(){
            string r="";
            for(int i = 0; i<grid.GetLength(0);i++){
                for(int j = 0; j<grid.GetLength(1);j++){
                    r+= this.grid[i,j].ToString() + "\n";
                }
            }
            return r;
        }
    }
    class Program
    {
        static String makeCSV(MDPGrid grid) {
            string csv = "";
            foreach (var row in grid.history)
            {
                foreach (var cell in row)
                {
                    csv += cell+",";
                }
                csv.Remove(csv.Length - 1);
                csv += "\n";
            }
            return csv;
        }
        static String makeCSVDirs(MDPGrid grid) {
            string csv = "";
            foreach (var row in grid.directionHistory)
            {
                foreach (var cell in row)
                {
                    csv += cell+",";
                }
                csv.Remove(csv.Length - 1);
                csv += "\n";
            }
            return csv;
        }
        public static bool convergence(MDPGrid MDPgrid){
            Node[,] grid = MDPgrid.grid;
            Node[,] Prevgrid = MDPgrid.prevGrid;
            for(int i = 0; i<MDPgrid.grid.GetLength(0);i++){
                for(int j = 0; j<MDPgrid.grid.GetLength(1);j++){
                    // Console.WriteLine(grid[i,j].direction+"ooooow"+ Prevgrid[i,j].direction);
                    if(grid[i,j].direction != Prevgrid[i,j].direction){
                        return false;
                    }
                }
            }
            return true;
        }
        static void Main(string[] args)
        {

            
            // Programatic way of calling the MDP
            // MDPGrid grid = new MDPGrid(20,20);
            // Position[] goalPositions = new Position[4]{new Position(9,9),new Position(0,14),new Position(15,14),new Position(19,19)};
            // Position[] wallPositions = new Position[13]{new Position(1,2),new Position(3,2),new Position(5,2),new Position(7,2),new Position(9,2),new Position(0,5),new Position(2,5),new Position(4,5),new Position(6,5),new Position(8,5),new Position(1,9),new Position(3,9),new Position(5,9)};
            // int[] goalValues = new int[4]{10,20,20,30};
            // grid.setWalls(wallPositions);
            // grid.setGoals(goalPositions, goalValues);
            Console.WriteLine("Initial State");
            // MDPGrid grid =new MDPGrid(15,10, "/Users/anak/Desktop/MDP-Excersise/input.csv");
            MDPGrid grid =new MDPGrid(100,100, "input_unity.csv");
            Console.WriteLine(grid.drawGrid());
            Console.WriteLine(grid.drawGridDirection());
            int iter = 0;
            grid.evalGrid();
            while(!convergence(grid)){
                grid.evalGrid();
                iter++;
                Console.WriteLine("iter: " +iter);
                // Console.WriteLine(grid.drawGrid());
                // Console.WriteLine(grid.drawGridDirection());
                // Console.WriteLine(convergence(grid));
            }
            Console.WriteLine("Final iter: " +iter);

            //                '''''''''''''Takes a lot to write
            // File.WriteAllText("valuedata.csv", makeCSV(grid));
            // File.WriteAllText("directiondata.csv", makeCSVDirs(grid));
            File.WriteAllText("policy.csv", grid.createPolicy());

        }
    }
}
