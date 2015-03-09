#pragma once
class a
{
protected:
	int haha;
virtual void foo();
};

class b :
	private a
{
void foo();
};

