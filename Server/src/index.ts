import express from 'express';

const server = express();
server.listen(6080);

server.get('/ping', (req, res) => {
    res.send(new Date().toString())
})

function getDate(add?: number) {
    const date = new Date();
    if (add) {
        return new Date(date.getTime() + add);
    }
    return date;
}

class EntitySyncState {
    public explosions = new Map<string, any>();
    public tanks = new Map<string, any>();
    public lifetime = new Map<string, Date>();

    public set(type: 'explosion' | 'tank', state: any) {
        if (type === 'explosion') {
            if (!this.explosions.has(state.id)) {
                this.lifetime.set(state.id, getDate(200));
                console.log('new explosion', state.id);
            }
            this.explosions.set(state.id, state);
        } else if (type === 'tank') {
            this.lifetime.set(state.id, getDate(5000));
            // state.position.x += 2;
            // state.targetPosition.x += 2;
            // state.id = state.id + '_copy';
            this.tanks.set(state.id, state);
        }
    }

    public clean() {
        const now = new Date().getTime();
        const deletes: string[] = [];
        for (const [id, expires] of Array.from(this.lifetime.entries())) {
            if (expires.getTime() <= now) {
                if (this.explosions.has(id)) this.explosions.delete(id);
                if (this.tanks.has(id)) this.tanks.delete(id);
                deletes.push(id);
            }
        }
        deletes.forEach(id => (
            this.lifetime.delete(id)
        ));
        return now;
    }

    public serialize() {
        this.clean();
        return {
            explosions: Array.from(this.explosions.values()),
            tanks: Array.from(this.tanks.entries()).map(([id, state]) => ({ id, state: JSON.stringify(state) })),
        }
    }
}

const serverState = new EntitySyncState();

server.post('/entity/sync', express.json(), (req, res) => {
    const payload = req.body;
    (payload.explosions || []).forEach(explosion => (
        serverState.set('explosion', explosion)
    ));
    if (payload.tank) {
        serverState.set('tank', payload.tank)
    }
    const serialized = serverState.serialize();
    serialized.tanks = serialized.tanks.filter(o => o.id != payload?.tank?.id);
    res.json({
        ...serialized,
    });
})