#include "test.h"


int a::b::foo(int i)
{
	return i++;
}


void a::d::lol()
{
	b u;

	int i = 10;
	for (i = 0; i < 100;)
		i = u.foo(i);
}
