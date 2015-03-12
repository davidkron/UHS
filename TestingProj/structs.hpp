#pragma once

struct st
{
int ii = 10;
virtual void foo();
};

struct st2 :
	private st
{
void foo();
};
