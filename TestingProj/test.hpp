#pragma once

class a
{

	class b
	{
	public:
		int foo(int i);
	};

	struct d :
		public b
	{
		void lol();
	};
};
