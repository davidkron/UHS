#pragma once
extern int boo;

struct obj
{
protected:
	virtual void foo();
};

template <typename T>
class d :
	private obj
{
const int stat = 10;
volatile T *myT;

static float calc()
{
return 1.1f;
}
};
