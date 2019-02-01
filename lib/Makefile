# Makefile for the MinGW suite for Windows
CC = gcc
THREADS = -pthread
OPT ?= -O2

XCFLAGS = -DSQLITE_ENABLE_FTS5 -DSQLITE_ENABLE_SESSION -DSQLITE_ENABLE_JSON1 -DSQLITE_ENABLE_UNLOCK_NOTIFY 
XCFLAGS += -DSQLITE_THREADSAFE=1 -DSQLITE_DEFAULT_MEMSTATUS=0 -DSQLITE_MAX_EXPR_DEPTH=0 -DSQLITE_OMIT_DEPRECATED
# -DSQLITE_ENABLE_EXPLAIN_COMMENTS -DSQLITE_DEBUG  -DSQLITE_ENABLE_DBSTAT_VTAB
# -DSQLITE_ENABLE_RTREE -DSQLITE_ENABLE_RBU

CFLAGS =  $(THREADS) $(OPT) $(W) $(XCFLAGS) -fPIC
LDFLAGS =  -lpthread # -ldl -lz # for UNIX or for Windows with pthread installed
LDFLAGS += -static -static-libgcc #-static-libstdc++ # for mingw

SOURCES = sqlite3.c
HEADERS = sqlite3.h
CFLAGS += -I.

DESTDIR = out/
CPUx ?= 32
prefix	= w$(CPUx)

NAME = spreads_sqlite3
LIBRARY = $(NAME).a
SHARED = $(NAME).dll #$(CPUx)

LIBOBJECTS = $(SOURCES:.c=.o)

all: $(LIBRARY) $(SHARED)

clean:
	rm -rf *.exe *.dll *.lib *.def *.[ao] *.[ls]o  $(LIBOBJECTS)

$(LIBRARY): $(LIBOBJECTS) $(HEADERS)
	rm -f $@
	$(AR) -rs $@ $(LIBOBJECTS)
	
%:	%.o
	$(CC) $(CFLAGS) $(LDFLAGS) $^ $(LDLIBS) -o $@
	
.c.o:
	$(CC) $(CFLAGS) -c $< -o $@
	
dll : $(SHARED)
	
$(SHARED):	$(SOURCES)
	$(CC)  $(CFLAGS) $(LDFLAGS) -shared $(SOURCES) -o $@ $(LDLIBS) $(THREADS) -s
	
install: all
	rm -rf $(DESTDIR)$(prefix)/; mkdir -p $(DESTDIR)$(prefix)/;
	
	rm -rf $(DESTDIR)$(prefix)/bin/; mkdir $(DESTDIR)$(prefix)/bin/;
	for f in $(NAME).dll $(NAME).def $(NAME).lib; do cp $$f $(DESTDIR)$(prefix)/bin; done
	
	rm -rf $(DESTDIR)$(prefix)/lib/; mkdir $(DESTDIR)$(prefix)/lib/;
	cp $(NAME).a $(DESTDIR)$(prefix)/lib; 
	
	rm -rf $(DESTDIR)$(prefix)/include/; mkdir $(DESTDIR)$(prefix)/include/;
	for f in $(HEADERS); do cp $$f $(DESTDIR)$(prefix)/include; done
	