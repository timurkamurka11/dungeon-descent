from pathlib import Path
import math, wave
import numpy as np
from PIL import Image, ImageDraw, ImageFilter
ROOT=Path(__file__).resolve().parents[1]
A=ROOT/'Assets/DungeonDescent/Art'
R=ROOT/'Assets/DungeonDescent/Resources'
(A/'Textures').mkdir(parents=True,exist_ok=True); (A/'Audio').mkdir(parents=True,exist_ok=True)
(R/'Textures').mkdir(parents=True,exist_ok=True); (R/'Audio').mkdir(parents=True,exist_ok=True)
rng=np.random.default_rng(240904)
N=1024

def noise(octaves=6):
    acc=np.zeros((N,N),dtype=np.float32); total=0
    for i in range(octaves):
        s=max(4,N//(2**(i+3)))
        small=rng.random((s,s),dtype=np.float32)
        im=Image.fromarray((small*255).astype(np.uint8)).resize((N,N),Image.Resampling.BICUBIC)
        arr=np.asarray(im,dtype=np.float32)/255
        w=0.55**i; acc+=arr*w; total+=w
    return acc/total

def save_tex(name, rgb):
    im=Image.fromarray(np.clip(rgb*255,0,255).astype(np.uint8),'RGB')
    im.save(A/'Textures'/name,optimize=True)
    im.save(R/'Textures'/name,optimize=True)

n=noise(7)
# Stone masonry with hand-cut joints and mineral variation
stone=np.zeros((N,N,3),float)
stone[:]=np.array([0.19,0.205,0.22])
stone*=0.72+0.5*n[...,None]
img=Image.fromarray(np.clip(stone*255,0,255).astype(np.uint8))
d=ImageDraw.Draw(img)
brick_h=128
for row,y in enumerate(range(0,N,brick_h)):
    offset=0 if row%2==0 else -128
    for x in range(offset,N,256):
        d.line((x,y,x+256,y),fill=(32,35,39),width=8)
        d.line((x,y,x,y+brick_h),fill=(31,34,38),width=8)
        d.line((x+4,y+8,x+250,y+8),fill=(88,91,92),width=2)
stone=np.asarray(img.filter(ImageFilter.GaussianBlur(.45)),dtype=np.float32)/255
save_tex('stone_albedo.png',stone)
# Normal map from luminance height
h=(stone.mean(axis=2)*255).astype(np.uint8)
hf=np.asarray(Image.fromarray(h).filter(ImageFilter.GaussianBlur(2)),dtype=np.float32)/255
gy,gx=np.gradient(hf)
norm=np.dstack((-gx*5,-gy*5,np.ones_like(gx)))
norm/=np.linalg.norm(norm,axis=2,keepdims=True)+1e-6
normal=(norm*.5+.5)
save_tex('stone_normal.png',normal)
# wood grain
y=np.linspace(0,1,N)[:,None]; x=np.linspace(0,1,N)[None,:]
grain=np.sin((x*45 + noise(4)*4)*math.pi)*.12 + noise(5)*.28
wood=np.zeros((N,N,3)); base=np.array([0.20,0.095,0.035]); wood[:]=base; wood*=.78+grain[...,None]+noise(5)[...,None]*.22
for seam in [0,256,512,768]: wood[:,max(0,seam-3):seam+3]*=.35
save_tex('wood_albedo.png',wood)
# hammered metal
m=noise(7); metal=np.dstack([.18+.2*m,.19+.21*m,.21+.24*m]);
for _ in range(160):
    cx,cy=rng.integers(0,N,2); rr=int(rng.integers(3,18)); yy,xx=np.ogrid[:N,:N]; mask=(xx-cx)**2+(yy-cy)**2<rr**2; metal[mask]*=rng.uniform(.65,.95)
save_tex('metal_albedo.png',metal)
# cloth
c=noise(4); weave=(np.sin(np.arange(N)[None,:]*math.pi/4)+np.sin(np.arange(N)[:,None]*math.pi/4))*.035
cloth=np.dstack([.17+.12*c+weave,.025+.035*c,.035+.045*c])
save_tex('cloth_albedo.png',cloth)
# moss
m=noise(7); moss=np.dstack([.035+.08*m,.09+.22*m,.035+.06*m])
save_tex('moss_albedo.png',moss)
# rune icon / logo
icon=Image.new('RGBA',(512,512),(0,0,0,0)); di=ImageDraw.Draw(icon)
di.ellipse((72,72,440,440),outline=(205,160,72,245),width=14)
di.polygon([(256,94),(330,244),(278,238),(365,418),(256,326),(147,418),(234,238),(182,244)],fill=(210,166,78,245))
di.ellipse((215,215,297,297),fill=(60,14,18,255),outline=(235,198,112,255),width=7)
icon=icon.filter(ImageFilter.GaussianBlur(.35)); icon.save(A/'Icons'/'dungeon_rune.png');
# audio synthesis
SR=22050

def write(name, samples):
    samples=np.asarray(samples,float)
    samples=np.tanh(samples*1.25)
    samples/=max(1.0,np.max(np.abs(samples))*1.02)
    pcm=(samples*32767).astype('<i2')
    for folder in [A/'Audio',R/'Audio']:
        with wave.open(str(folder/name),'wb') as w:
            w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR); w.writeframes(pcm.tobytes())

def tsec(s): return np.arange(int(SR*s))/SR

def drone(duration, roots, noise_amt=.03, pulse=.0, bright=.0):
    t=tsec(duration); out=np.zeros_like(t)
    for i,f in enumerate(roots):
        out += (0.22/(i+1))*np.sin(2*np.pi*f*t + np.sin(t*.11+i)*.7)
        out += (0.08/(i+1))*np.sin(2*np.pi*f*2.01*t)
    if bright: out += bright*np.sin(2*np.pi*roots[0]*4*t)*(0.5+0.5*np.sin(t*.23))
    if pulse: out *= .72+.28*(.5+.5*np.sin(2*np.pi*pulse*t))
    n=rng.normal(0,1,len(t)); n=np.convolve(n,np.ones(120)/120,mode='same')
    out += n*noise_amt
    fade=np.minimum(1,np.minimum(t/2,(duration-t)/2)); return out*fade*.75
write('safe_room.wav',drone(28,[55,82.41,110],.025,.08,.05))
write('exploration.wav',drone(30,[41.2,61.7,82.4],.045,.055,.02))
write('combat.wav',drone(24,[49,73.4,98],.065,1.8,.08)+.12*np.sin(2*np.pi*2.8*tsec(24))*np.sin(2*np.pi*110*tsec(24)))
write('boss.wav',drone(28,[36.7,55,73.4],.08,1.35,.11)+.16*np.sign(np.sin(2*np.pi*1.35*tsec(28)))*np.sin(2*np.pi*73.4*tsec(28)))
# ambience and sfx
for name,dur,kind in [('fireplace.wav',18,'fire'),('dungeon_wind.wav',20,'wind')]:
    t=tsec(dur); n=rng.normal(0,1,len(t));
    if kind=='fire':
        crack=np.convolve(n,np.ones(12)/12,mode='same')*.18
        pops=np.zeros_like(t)
        for _ in range(70):
            i=int(rng.integers(0,len(t)-400)); L=int(rng.integers(60,350)); pops[i:i+L]+=np.exp(-np.linspace(0,7,L))*rng.uniform(.2,.7)
        write(name,crack+pops)
    else:
        smooth=np.convolve(n,np.ones(500)/500,mode='same')*2.4
        write(name,smooth*(.55+.45*np.sin(t*.19)**2))

def sweep(dur,f0,f1,amp=.8,noise=.0):
    t=tsec(dur); phase=2*np.pi*(f0*t+(f1-f0)/(2*dur)*t*t); env=np.sin(np.clip(t/dur,0,1)*np.pi)**1.2
    return np.sin(phase)*env*amp+rng.normal(0,noise,len(t))*env
write('sword_swing.wav',sweep(.42,190,48,.65,.08))
write('sword_impact.wav',sweep(.34,75,32,.7,.22))
write('door_creak.wav',sweep(2.2,38,21,.55,.09)+.1*np.sin(2*np.pi*73*tsec(2.2))*(1-tsec(2.2)/2.2))
write('loot.wav',sweep(.75,440,990,.45,.02)+sweep(.75,660,1320,.25,0))
write('heal.wav',sweep(1.05,260,720,.35,.018)+.18*np.sin(2*np.pi*880*tsec(1.05))*np.sin(np.pi*tsec(1.05)/1.05))
print('generated art/audio')
