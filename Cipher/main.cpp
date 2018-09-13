#include <boost/multiprecision/cpp_int.hpp>

using namespace boost::multiprecision;
using namespace std;

int128_t c;
int128_t K;
int n;
int128_t seriesInit[3];     //(2,1,c-2) c nieznane

int matS=3;
//  [0][0]  [0][1]  [0][2]
//  [1][0]  [1][1]  [1][2]
//  [2][0]  [2][1]  [2][2]
typedef struct mat3
{
    int128_t v[3][3];
} mat3;

mat3 step[64];

void matrixMult(mat3* matOut, mat3 matA, mat3 matB)
{
    int128_t sum;
    for(int i=0; i<matS; i++)
        for(int j=0; j<matS; j++)
        {3;
            sum=0;
            for(int k=0; k<matS; k++) sum+=matA.v[i][k]*matB.v[k][j];
            (*matOut).v[i][j]=sum%K;
        }
}
void setMat3(mat3* matOut, int128_t* initVal)
{
    for(int i=0; i<matS*matS; i++)
    {
        (*matOut).v[i/matS][i%matS]=initVal[i];
    }
}

void printMat3(mat3 matIn)
{
    for(int i=0; i<matS; i++)
    {
        for(int j=0; j<matS; j++)
        {
            cout << matIn.v[i][j] << "\t";
        }
    cout << endl;
    }
}

int128_t sol(int128_t x)
{
    if(x<=1) return 1;
    if(x==2) return 2;
    x=x-2;
    int128_t l=1;  //wyluskiwacz bitu z x, patrza 6 linijek nizej - x&l === x (BITOWE AND) l
    mat3 jump;
    int128_t jumpInit[9]={1,0,0,0,1,0,0,0,1};
    setMat3(&jump,jumpInit);
    for(int k=0; l<=x; k++)
    {
        if(x&l) matrixMult(&jump,jump,step[k]);
        l=l<<1;
    }
    int128_t temp= (jump.v[0][0]*seriesInit[0]+
                    jump.v[0][1]*seriesInit[1]+
                    jump.v[0][2]*seriesInit[2])%K;
    if(temp<0) temp=K+temp;         //modulo...
    return temp;

}
int main()
{
    cin >> c >> K >> n;
    seriesInit[0]=2;seriesInit[1]=1;seriesInit[2]=2-c;
    int128_t stepZeroInitValues[9]={c,1,1,1,0,0,0,0,1}; //Zero like in step by 2^0, ie step by one
    setMat3(&step[0],stepZeroInitValues);
    for(int i=1; i<64; i++)
        matrixMult(&step[i],step[i-1],step[i-1]);
    int128_t x;
    for(int i=0; i<n; i++)
    {
        cin >> x;
        cout << sol(x) << endl;
    }
    cout << endl;
}
