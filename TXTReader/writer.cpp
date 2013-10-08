#include<stdio.h>

int main(){
	freopen("a.txt","w",stdout);
	for (int i=0;i<10000000;++i) printf("%ld",i);
	fclose(stdout);
}