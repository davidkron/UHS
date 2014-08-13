#include "mess.h"
int global;
const int fynal = 10;

void messy()
{

}

void boo::messier()
{
	::messy();
	::messy();
}

int boo::calc()
{
	return fynal + 1;
}
