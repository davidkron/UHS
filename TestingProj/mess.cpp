#include "mess.hpp"
int global;
const int fynal = 10;

void messy()
{

}

void name_space::messier()
{
	::messy();
	::messy();
}

int name_space::calc()
{
	return fynal + 1;
}
