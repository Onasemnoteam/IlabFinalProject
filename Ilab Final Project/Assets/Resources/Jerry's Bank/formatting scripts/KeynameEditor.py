f = open("SFZ.txt", "r");
f2 = open("output.txt", "w");
keys = ["a","a#", "b", "c","c#","d", "d#", "e","f","f#", "g", "g#"];
for line in f:
    if (line[0:6] == "lokey=" or line[0:6] == "hikey=" or line[0:4] == "key=" or line[0:16]=="pitch_keycenter="):
        if (line[-2]==" "):
            line = line[:-2] + line[-1:];
        mult = int(line[-2:-1]);
        #print(mult)
        
        note = line[line.find("=")+1:-2];
        modi = keys.index(note);
        f2.write(line[0:line.find("=")+1] +str( (mult*12+modi))+"\n");
    else:
        f2.write(line);
f2.close();
